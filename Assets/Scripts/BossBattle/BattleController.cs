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
    
    // ▼▼▼ [수정] 연출 매니저 변수 추가 ▼▼▼
    [Header("연출 매니저")]
    public DiceAnimationManager diceAnimationManager; 

    [Header("승리 연출")]
    public TextMeshProUGUI victoryText;
    public string nextStageName = "Stage2";

    [Header("넉백 설정")]
    public float knockbackPower = 0.1f;

    // --- 비공개 변수들 ---
    private Vector3 originalPlayerClashPos; // 초기 위치 저장용 변수
    private Vector3 originalBossClashPos;   // 초기 위치 저장용 변수
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

        // 전투 시작 시, clashPosition의 초기 위치를 저장
        if (playerClashPosition != null) originalPlayerClashPos = playerClashPosition.position;
        if (bossClashPosition != null) originalBossClashPos = bossClashPosition.position;

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

        if (boss != null && boss.healthBarUIParent != null)
        {
            boss.healthBarUIParent.SetActive(true);
            boss.UpdateHealthUI();
        }

        player.SortDeckByCost();
        boss.SortDeckByCost();

        StartCoroutine(SetupNewTurn());
        yield return null;
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

        int clashCount = playerActionQueueUI.Count;
        for (int i = 0; i < clashCount; i++)
        {
            if (i >= bossActionQueue.Count) break;

            playerVisuals.FaceOpponent(boss.transform);
            bossVisuals.FaceOpponent(player.transform);

            StartCoroutine(playerVisuals.MoveToPosition(playerClashPosition.position, 0.2f));
            StartCoroutine(bossVisuals.MoveToPosition(bossClashPosition.position, 0.2f));
            yield return new WaitForSeconds(0.2f);

            CombatPage playerPage = playerActionQueueUI[i].assignedPage;
            CombatPage bossPage = bossActionQueue[i];

            // ▼▼▼ [수정] ClashManager 대신 DiceAnimationManager를 호출 ▼▼▼
            if (diceAnimationManager != null)
            {
                // 1. 주사위 UI 생성
                diceAnimationManager.SetupDiceVisuals(playerPage, bossPage);

                // 2. 애니메이션 및 데미지 계산 코루틴 실행 (끝날 때까지 대기)
                yield return StartCoroutine(diceAnimationManager.AnimateClashSequence(
                    player, playerPage,
                    boss, bossPage
                ));
            }
            else
            {
                // (혹시 연결 안 했을 경우 대비) 기존 로직 실행
                Debug.LogWarning("DiceAnimationManager가 연결되지 않아 즉시 계산합니다.");
                ClashManager.ResolveClash(player, playerPage, boss, bossPage);
            }
            // ▲▲▲ 수정 완료 ▲▲▲


            // [수정] DiceAnimationManager가 이미 대기하므로 추가 대기 시간(1.0f) 제거
            // yield return new WaitForSeconds(1.0f); 

            if (player.currentHp <= 0 || boss.currentHp <= 0)
            {
                // 주사위 연출 중 캐릭터가 죽었다면, 남은 주사위 연출을 취소하기 위해
                // DiceAnimationManager의 주사위 비주얼을 즉시 정리합니다.
                if (diceAnimationManager != null)
                {
                    diceAnimationManager.ClearDiceVisuals();
                }
                break;
            }
        }

        Debug.Log("캐릭터들을 원래 위치로 복귀...");
        if (player.currentHp > 0)
        {
            StartCoroutine(playerVisuals.ReturnToHomePosition(0.5f));
        }
        if (boss.currentHp > 0)
        {
            StartCoroutine(bossVisuals.ReturnToHomePosition(0.5f));
        }
        yield return new WaitForSeconds(0.5f);

        if (mainCameraController != null)
        {
            yield return StartCoroutine(mainCameraController.ZoomOut());
        }

        Debug.Log("======== 페이즈 종료 ========");

        foreach (var actionCardUI in playerActionQueueUI)
        {
            player.SetCardCooldown(actionCardUI.assignedPage);
        }
        foreach (var bossPage in bossActionQueue)
        {
            boss.SetCardCooldown(bossPage);
        }

        StartCoroutine(SetupNewTurn());
    }

    IEnumerator SetupNewTurn()
    {
        // 턴 시작 전에 이미 전투가 끝났는지 확인
        if (player.currentHp <= 0 || boss.currentHp <= 0)
        {
            yield break; // 승/패 연출이 진행 중이므로 새 턴 시작 안 함
        }

        Debug.Log("======== 새로운 턴 시작 ========");

        // 매 턴이 시작될 때, clashPosition을 저장해둔 초기 위치로 리셋
        if (playerClashPosition != null) playerClashPosition.position = originalPlayerClashPos;
        if (bossClashPosition != null) bossClashPosition.position = originalBossClashPos;

        foreach (var ui in playerActionQueueUI)
        {
            if (ui != null) Destroy(ui.gameObject);
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

    // CharacterStats가 호출하여 clashPosition을 이동시키는 함수
    public void ApplyClashPointKnockback(CharacterStats character, float distance)
    {
        CharacterVisuals visuals = character.GetComponent<CharacterVisuals>();
        if (visuals == null || visuals.homeTransform == null) return;

        // 넉백 방향과 거리를 계산
        Vector3 knockbackDirection = (visuals.homeTransform.position - character.transform.position).normalized;
        Vector3 knockbackVector = knockbackDirection * distance;

        // 플레이어와 보스의 ClashPosition을 둘 다 똑같은 양만큼 이동
        if (playerClashPosition != null)
        {
            playerClashPosition.position += knockbackVector;
        }
        if (bossClashPosition != null)
        {
            bossClashPosition.position += knockbackVector;
        }
    }

    // CharacterStats가 호출하여 승/패 연출을 시작하는 함수
    public void OnCharacterDefeated(CharacterStats defeatedCharacter)
    {
        if (defeatedCharacter == boss)
        {
            StartCoroutine(VictorySequence());
        }
        else if (defeatedCharacter == player)
        {
            Debug.Log("플레이어가 패배했습니다...");
            // (추후 여기에 패배 로직 추가)
        }
    }

    private IEnumerator VictorySequence()
    {
        Debug.Log("보스 처치! 승리했습니다!");

        if (boss != null && boss.healthBarUIParent != null)
        {
            boss.healthBarUIParent.SetActive(false);
        }

        yield return new WaitForSeconds(1.0f);

        if (player != null && boss != null && boss.deck != null)
        {
            Debug.Log("보스의 덱 카드를 획득합니다...");
            
            foreach (CombatPage card in boss.deck)
            {
                player.AddCardToCollection(card);
            }
            Debug.Log($"[BattleController] 총 {boss.deck.Count}개의 카드를 획득했습니다. (현재 총 소유 카드: {player.cardCollection.Count}개)");
        }

        if (boss != null)
        {
            boss.gameObject.SetActive(false);
        }

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

    // (이하 카드 UI 관련 함수들은 이전 버전과 동일)
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
            if (card != null) card.UpdateState(player);
        }
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
}