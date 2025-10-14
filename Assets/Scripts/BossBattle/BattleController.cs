using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleController : MonoBehaviour
{
    [Header("캐릭터")]
    public CharacterStats player;
    public CharacterStats boss;

    [Header("UI 패널")]
    public Transform handPanel;
    public Transform actionSlotsPanel;

    [Header("UI 요소")]
    public GameObject cardPrefab;
    public Button nextButton;
    public Button previousButton;
    public Button viewEnemyDeckButton;
    public TextMeshProUGUI deckViewModeText;

    [Header("전투 위치")]
    public Transform playerClashPosition;
    public Transform bossClashPosition;

    [Header("카메라 컨트롤러")]
    public CameraController mainCameraController;
    
    [Header("승리 연출")]
    public TextMeshProUGUI victoryText;
    public string nextStageName = "Stage2";

    private int currentPage = 0;
    private const int cardsPerPage = 3;
    private bool isPlayerActionsConfirmed = false;
    private bool isViewingBoss = false;
    private List<CardUI> displayedCardUIs = new List<CardUI>();
    private List<CardUI> playerActionQueueUI = new List<CardUI>();
    private List<CombatPage> bossActionQueue = new List<CombatPage>();

    public bool IsViewingBossDeck() => isViewingBoss;

    void Start()
    {
        nextButton.onClick.AddListener(ShowNextGroup);
        previousButton.onClick.AddListener(ShowPreviousGroup);
        viewEnemyDeckButton.onClick.AddListener(ToggleDeckViewMode);
        
        StartCoroutine(SetupBattle());
    }

    void Update()
    {
        if (isPlayerActionsConfirmed || isViewingBoss) return;

        if (playerActionQueueUI.Count > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            isPlayerActionsConfirmed = true;
            handPanel.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            previousButton.gameObject.SetActive(false);
            viewEnemyDeckButton.gameObject.SetActive(false);
            deckViewModeText.gameObject.SetActive(false);

            StartCoroutine(StartClashPhase());
        }
    }

    IEnumerator SetupBattle()
    {
        if (player == null)
        {
            if (PlayerController.Instance != null)
            {
                player = PlayerController.Instance.GetComponent<CharacterStats>();
            }
        }

        if (player != null)
        {
            player.InitializeFromPlayerScripts();
        }
        
        player.SortDeckByCost();
        boss.SortDeckByCost();
        
        StartCoroutine(SetupNewTurn());
        yield return null;
    }

    public void ToggleDeckViewMode()
    {
        isViewingBoss = !isViewingBoss;
        currentPage = 0;
        DisplayCurrentGroupCards();
    }

    void DisplayCurrentGroupCards()
    {
        CharacterStats currentCharacter = isViewingBoss ? boss : player;
        deckViewModeText.text = isViewingBoss ? "Boss Deck" : "Player Deck";
        
        foreach (Transform child in handPanel) Destroy(child.gameObject);
        displayedCardUIs.Clear();

        int startIndex = currentPage * cardsPerPage;
        int endIndex = Mathf.Min(startIndex + cardsPerPage, currentCharacter.deck.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            CombatPage page = currentCharacter.deck[i];
            if (!isViewingBoss && playerActionQueueUI.Any(cardUI => cardUI.assignedPage == page)) continue;

            GameObject cardObject = Instantiate(cardPrefab, handPanel);
            CardUI cardUI = cardObject.GetComponent<CardUI>();
            bool isRevealed = !isViewingBoss || boss.revealedCards.Contains(page);
            cardUI.Setup(page, this, currentCharacter, isRevealed);
            displayedCardUIs.Add(cardUI);
        }
        UpdateArrowButtons();
    }

    public void ShowNextGroup()
    {
        CharacterStats currentCharacter = isViewingBoss ? boss : player;
        int maxPage = (currentCharacter.deck.Count - 1) / cardsPerPage;
        if (currentPage < maxPage)
        {
            currentPage++;
            DisplayCurrentGroupCards();
        }
    }

    public void ShowPreviousGroup()
    {
        if (currentPage > 0)
        {
            currentPage--;
            DisplayCurrentGroupCards();
        }
    }

    void UpdateArrowButtons()
    {
        CharacterStats currentCharacter = isViewingBoss ? boss : player;
        previousButton.interactable = (currentPage > 0);
        int maxPage = (currentCharacter.deck.Count - 1) / cardsPerPage;
        nextButton.interactable = (currentPage < maxPage);
    }

    public void SelectCardForAction(CardUI selectedCardUI)
    {
        if (isViewingBoss) return;
        CombatPage page = selectedCardUI.assignedPage;
        if (playerActionQueueUI.Count >= 3 || player.currentLight < page.lightCost || !player.IsCardUsable(page)) return;

        player.currentLight -= page.lightCost;
        playerActionQueueUI.Add(selectedCardUI);
        selectedCardUI.transform.SetParent(actionSlotsPanel);
        selectedCardUI.SetSelected(true);
        displayedCardUIs.Remove(selectedCardUI);
        
        if (playerActionQueueUI.Count == 1) Debug.Log("카드를 선택했습니다. 스페이스바를 눌러 전투를 시작하세요.");
        foreach (var card in displayedCardUIs)
        {
            if(card != null) card.UpdateState(player);
        }
    }
    
    IEnumerator StartClashPhase()
    {
        isPlayerActionsConfirmed = false;

        CharacterVisuals playerVisuals = player.GetComponent<CharacterVisuals>();
        CharacterVisuals bossVisuals = boss.GetComponent<CharacterVisuals>();

        if (mainCameraController != null)
        {
            yield return StartCoroutine(mainCameraController.ZoomIn());
        }
        
        playerVisuals.FaceOpponent(boss.transform);
        bossVisuals.FaceOpponent(player.transform);

        Debug.Log("캐릭터들을 전투 위치로 이동...");
        StartCoroutine(playerVisuals.MoveToPosition(playerClashPosition.position, 0.5f));
        StartCoroutine(bossVisuals.MoveToPosition(bossClashPosition.position, 0.5f));
        yield return new WaitForSeconds(0.5f);

        int clashCount = playerActionQueueUI.Count;
        for (int i = 0; i < clashCount; i++)
        {
            if (i >= bossActionQueue.Count) break;
            
            Debug.Log($"--------- [ {i + 1}번째 합 ] ---------");
            CombatPage playerPage = playerActionQueueUI[i].assignedPage;
            CombatPage bossPage = bossActionQueue[i];

            ClashManager.ResolveClash(player, playerPage, boss, bossPage);
            
            yield return new WaitForSeconds(1.5f);
        }

        playerVisuals.FaceOpponent(boss.transform);
        bossVisuals.FaceOpponent(player.transform);
        
        Debug.Log("캐릭터들을 원래 위치로 복귀...");
        StartCoroutine(playerVisuals.ReturnToHomePosition(0.5f));
        StartCoroutine(bossVisuals.ReturnToHomePosition(0.5f));
        yield return new WaitForSeconds(0.5f);

        if (mainCameraController != null)
        {
            yield return StartCoroutine(mainCameraController.ZoomOut());
        }

        Debug.Log("======== 페이즈 종료 ========");
        
        foreach(var actionCardUI in playerActionQueueUI)
        {
            player.SetCardCooldown(actionCardUI.assignedPage);
        }
        foreach(var bossPage in bossActionQueue)
        {
            boss.SetCardCooldown(bossPage);
        }

        StartCoroutine(SetupNewTurn());
    }

    IEnumerator SetupNewTurn()
    {
        Debug.Log("======== 새로운 턴 시작 ========");
        
        foreach (var ui in playerActionQueueUI)
        {
            if(ui != null) Destroy(ui.gameObject);
        }
        playerActionQueueUI.Clear();
        bossActionQueue.Clear();

        player.OnNewTurnStart();
        boss.OnNewTurnStart();
        BossSelectsActions();

        isPlayerActionsConfirmed = false;
        handPanel.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        previousButton.gameObject.SetActive(true);
        viewEnemyDeckButton.gameObject.SetActive(true);
        deckViewModeText.gameObject.SetActive(true);
        
        isViewingBoss = false;
        currentPage = 0;
        DisplayCurrentGroupCards();
        yield return null;
    }

    private void BossSelectsActions()
    {
        bossActionQueue.Clear();
        var usableCards = boss.deck.Where(p => boss.IsCardUsable(p)).ToList();
        var shuffledUsableCards = usableCards.OrderBy(x => Random.value).ToList();
        int availableLight = boss.currentLight;
        int cardsSelected = 0;

        foreach (var page in shuffledUsableCards)
        {
            if (availableLight >= page.lightCost && cardsSelected < 3)
            {
                bossActionQueue.Add(page);
                availableLight -= page.lightCost;
                cardsSelected++;
            }
            if (cardsSelected >= 3) break;
        }
        Debug.Log($"보스가 이번 턴의 행동을 랜덤으로 결정했습니다. (총 {bossActionQueue.Count}개)");
    }
    
    public void OnCharacterDefeated(CharacterStats defeatedCharacter)
    {
        if (defeatedCharacter == boss)
        {
            StartCoroutine(VictorySequence());
        }
        else if (defeatedCharacter == player)
        {
            Debug.Log("플레이어가 패배했습니다...");
        }
    }

    private IEnumerator VictorySequence()
    {
        Debug.Log("보스 처치! 승리했습니다!");

        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(10f);

        if (BossGameManager.Instance != null)
        {
            BossGameManager.Instance.ChangeState(GameState.Exploration);
        }

        SceneManager.LoadScene(nextStageName);
    }
}