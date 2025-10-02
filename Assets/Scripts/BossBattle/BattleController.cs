using UnityEngine;
using UnityEngine.UI;
using System.Collections; // <--- 이렇게 수정하세요.
using System.Collections.Generic;
using System.Linq;
using TMPro;

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

    // --- 페이지네이션 변수 ---
    private int currentPage = 0;
    private const int cardsPerPage = 3;

    // --- 턴 진행 및 상태 변수 ---
    private bool isPlayerActionsConfirmed = false;
    private bool isViewingBoss = false;
    private List<CardUI> displayedCardUIs = new List<CardUI>();
    private List<CardUI> playerActionQueueUI = new List<CardUI>();
    private List<CombatPage> bossActionQueue = new List<CombatPage>();

    // 외부에서 현재 덱 보기 모드를 확인할 수 있는 함수
    public bool IsViewingBossDeck()
    {
        return isViewingBoss;
    }

    void Start()
    {
        // UI 버튼들에 기능 연결
        nextButton.onClick.AddListener(ShowNextGroup);
        previousButton.onClick.AddListener(ShowPreviousGroup);
        viewEnemyDeckButton.onClick.AddListener(ToggleDeckViewMode);
        
        // 전투 준비 시작
        StartCoroutine(SetupBattle());
    }

    void Update()
    {
        // 플레이어의 행동이 확정되었거나, 보스 덱을 보는 중이면 Space 입력 무시
        if (isPlayerActionsConfirmed || isViewingBoss) return;

        // 플레이어가 카드를 1장 이상 선택했고, 스페이스바를 누르면 전투를 시작
        if (playerActionQueueUI.Count > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            isPlayerActionsConfirmed = true;
            Debug.Log("스페이스바 입력! 전투를 실행합니다.");

            // 더 이상 카드 관련 UI를 조작할 수 없도록 비활성화
            foreach (var card in displayedCardUIs)
            {
                card.UpdateState(player);
            }
            handPanel.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            previousButton.gameObject.SetActive(false);
            viewEnemyDeckButton.gameObject.SetActive(false);
            deckViewModeText.gameObject.SetActive(false);

            StartCoroutine(StartClashPhase());
        }
        
    }

    // 최초 전투 준비
    IEnumerator SetupBattle()
    {
        player.SortDeckByCost();
        boss.SortDeckByCost();
        
        StartCoroutine(SetupNewTurn());
        yield return null;
    }

    // 플레이어/보스 덱 보기 모드 전환
    public void ToggleDeckViewMode()
    {
        isViewingBoss = !isViewingBoss;
        currentPage = 0; // 페이지를 처음으로 리셋
        DisplayCurrentGroupCards();
    }

    // 현재 페이지에 맞는 카드들을 UI에 표시
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
            
            if (!isViewingBoss && playerActionQueueUI.Any(cardUI => cardUI.assignedPage == page))
            {
                continue;
            }

            GameObject cardObject = Instantiate(cardPrefab, handPanel);
            CardUI cardUI = cardObject.GetComponent<CardUI>();
            bool isRevealed = !isViewingBoss || boss.revealedCards.Contains(page);
            
            cardUI.Setup(page, this, currentCharacter, isRevealed);
            displayedCardUIs.Add(cardUI);
        }
        UpdateArrowButtons();
    }

    // 다음 카드 그룹 표시
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

    // 이전 카드 그룹 표시
    public void ShowPreviousGroup()
    {
        if (currentPage > 0)
        {
            currentPage--;
            DisplayCurrentGroupCards();
        }
    }

    // 화살표 버튼 활성화/비활성화
    void UpdateArrowButtons()
    {
        CharacterStats currentCharacter = isViewingBoss ? boss : player;
        previousButton.interactable = (currentPage > 0);
        int maxPage = (currentCharacter.deck.Count - 1) / cardsPerPage;
        nextButton.interactable = (currentPage < maxPage);
    }

    // 플레이어가 카드를 클릭했을 때 호출
    public void SelectCardForAction(CardUI selectedCardUI)
    {
         Debug.Log($"카드 클릭 시도: {selectedCardUI.assignedPage.pageName}");
        if (isViewingBoss) return;
        
        CombatPage page = selectedCardUI.assignedPage;

        if (playerActionQueueUI.Count >= 3) return;
        if (player.currentLight < page.lightCost) return;
        if (!player.IsCardUsable(page)) return;

        player.currentLight -= page.lightCost;
        playerActionQueueUI.Add(selectedCardUI);
    
        selectedCardUI.transform.SetParent(actionSlotsPanel);
    
         // ★★★ 새로 추가: 선택된 카드의 하이라이트를 켭니다.
        selectedCardUI.SetSelected(true);
    
        displayedCardUIs.Remove(selectedCardUI);
         // Destroy 코드는 이전에 삭제했으므로 그대로 둡니다.
        //Destroy(selectedCardUI.gameObject);

        if (playerActionQueueUI.Count == 1)
        {
            Debug.Log("카드를 선택했습니다. 순서대로 더 선택하거나, 스페이스바를 눌러 전투를 시작하세요.");
        }

        foreach (var card in displayedCardUIs)
        {
            if(card != null) card.UpdateState(player);
        }
    }
    
    // 전투 실행
    IEnumerator StartClashPhase()
    {
        CharacterVisuals playerVisuals = player.GetComponent<CharacterVisuals>();
        CharacterVisuals bossVisuals = boss.GetComponent<CharacterVisuals>();

        int clashCount = playerActionQueueUI.Count;
        for (int i = 0; i < clashCount; i++)
        {
            if (i >= bossActionQueue.Count) break;

            Debug.Log($"--------- [ {i + 1}번째 합 ] ---------");
            CombatPage playerPage = playerActionQueueUI[i].assignedPage;
            CombatPage bossPage = bossActionQueue[i];

            yield return StartCoroutine(playerVisuals.MoveToPosition(playerClashPosition.position, 0.5f));
            yield return StartCoroutine(bossVisuals.MoveToPosition(bossClashPosition.position, 0.5f));

            yield return new WaitForSeconds(0.5f);
            ClashManager.ResolveClash(player, playerPage, boss, bossPage);
            yield return new WaitForSeconds(1.5f);

            yield return StartCoroutine(playerVisuals.ReturnToHomePosition(0.5f));
            yield return StartCoroutine(bossVisuals.ReturnToHomePosition(0.5f));
            yield return new WaitForSeconds(0.5f);
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

    // 새로운 턴 준비
    IEnumerator SetupNewTurn()
    {
        Debug.Log("======== 새로운 턴 시작 ========");
        
        //foreach (var ui in playerActionQueueUI) Destroy(ui.gameObject);
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

    // 보스 AI: 행동 자동 선택
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
}