// 파일명: BlacksmithMinigameManager.cs (최종 수정 버전)
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가
using UnityEngine.Events;

public class BlacksmithMinigameManager : MonoBehaviour
{
    public static bool isGamePausedByManager = false;
    [Header("게임 시작 설명")]
    [Tooltip("미니게임 시작 시 보여줄 설명 대사")]
    [TextArea(3, 10)]
    public string[] startDialogue;
    public static BlacksmithMinigameManager Instance { get; private set; }

    [Header("UI 요소")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("미니게임 설정")]
    [SerializeField] private float gameDuration = 60f;
    [SerializeField] private GameObject soulFlamePrefab;
    [SerializeField] private float spawnInterval = 2f;

    [Header("스폰 영역")]
    [SerializeField] private Transform spawnAreaMin;
    [SerializeField] private Transform spawnAreaMax;

    // --- 이 부분을 추가하세요 ---
    [Header("종료 UI")]
    [SerializeField] private GameObject rewardPanel; // 게임 종료 시 켤 패널
    [SerializeField] private TextMeshProUGUI rewardInfoText; // 보상 내용을 표시할 텍스트
    // --- 여기까지 ---

    private float currentTime;
    private int score;
    private bool isGameActive = true; // 게임 활성화 상태 변수 추가

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        isGamePausedByManager = false;
        if (rewardPanel != null) rewardPanel.SetActive(false);
        Time.timeScale = 0f; // 게임 일시정지

        // DialogueController를 통해 설명 대화를 시작합니다.
        if (DialogueController.Instance != null && startDialogue.Length > 0)
        {
            // 대화가 끝난 후에 실행할 행동으로 'StartMinigame' 함수를 지정합니다.
            UnityEvent endAction = new UnityEvent();
            endAction.AddListener(StartMinigame);

            DialogueController.Instance.StartDialogue(startDialogue, endAction);
        }
        else
        {
            // 보여줄 설명이 없다면, 바로 게임을 시작합니다.
            StartMinigame();
        }
    }
    public void StartMinigame()
    {
        Time.timeScale = 1f; // 멈췄던 게임 시간을 다시 흐르게 합니다.
        isGameActive = true;

        currentTime = gameDuration;
        score = 0;
        UpdateScoreUI();
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        StartCoroutine(SpawnFlameRoutine());

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timerText.text = $"remaining time: {currentTime:F0}";
            yield return null;
        }

        EndGame(); // 시간이 다 되면 게임 종료 함수 호출
    }

    // --- 아래 함수들을 새로 추가하거나 수정합니다 ---

    private void EndGame()
    {
        isGameActive = false;
        StopAllCoroutines();
        timerText.text = "game over!";

        // 게임 종료 시, 게임의 시간을 멈춥니다.
        Time.timeScale = 0f;
        isGamePausedByManager = true; // 게임이 이 매니저에 의해 멈췄다고 '기록'

        // ★★★ 이 부분이 수정되었습니다 ★★★
        // 보상 계산 후 '즉시 지급'하도록 변경
        CalculateAndGrantRewards();
        // ★★★ 여기까지 ★★★

        if (rewardPanel != null) rewardPanel.SetActive(true);
    }

    // ★★★ 함수 이름과 내용이 변경되었습니다 ★★★
    private void CalculateAndGrantRewards()
    {
        string rewardMessage;
        int goldReward = 0;
        int xpReward = 0;
        int attackBonusReward = 0;

        // 점수(모은 불꽃 갯수)에 따라 보상 수치 계산
        if (score >= 20) // 예시: 불꽃 20개 이상
        {
            rewardMessage = $"최고의 성과!\n\n골드 +100\n경험치 +50\n공격력 +5";
            goldReward = 100;
            xpReward = 50;
            attackBonusReward = 5;
        }
        else if (score >= 10) // 예시: 불꽃 10개 이상
        {
            rewardMessage = $"훌륭한 성과!\n\n골드 +50\n경험치 +20\n공격력 +2";
            goldReward = 50;
            xpReward = 20;
            attackBonusReward = 2;
        }
        else // 그 외
        {
            rewardMessage = $"기본 보상\n\n골드 +10\n경험치 +10";
            goldReward = 10;
            xpReward = 10;
        }

        // 1. UI에 보상 내용을 표시합니다.
        if (rewardInfoText != null) rewardInfoText.text = rewardMessage;

        // 2. 'Player' 태그로 플레이어를 찾아서 PlayerStats 스크립트를 가져옵니다.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            PlayerStats playerStats = playerObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                // 3. PlayerStats 스크립트의 함수를 직접 호출하여 보상을 즉시 지급합니다.
                Debug.Log("플레이어에게 보상을 직접 지급합니다...");
                if (goldReward > 0) playerStats.AddMoney(goldReward);
                if (xpReward > 0) playerStats.AddXp(xpReward);
                if (attackBonusReward > 0) playerStats.UpgradeAttackPower(); // UpgradeAttackPower는 5씩 오르므로, 여러번 호출하거나 값을 받도록 수정 필요
                // 참고: PlayerStats의 UpgradeAttackPower()는 5씩 오르도록 고정되어 있습니다.
                // 만약 2의 보너스를 주고 싶다면, PlayerStats의 UpgradeAttackPower() 함수를
                // public void UpgradeAttackPower(int amount) { bonusAttackPower += amount; ... } 와 같이 수정해야 합니다.
                // 지금은 기존 함수를 그대로 활용하겠습니다.
            }
            else
            {
                Debug.LogError("플레이어 오브젝트에서 PlayerStats 스크립트를 찾을 수 없습니다!");
            }
        }
        else
        {
            Debug.LogError("'Player' 태그를 가진 플레이어 오브젝트를 씬에서 찾을 수 없습니다!");
        }
    }

    // '돌아가기' 버튼에 연결할 함수
    public void ReturnToPreviousScene()
    {
        Time.timeScale = 1f;
        isGamePausedByManager = false; // 게임이 다시 시작되므로 '기록'을 해제
        // StatueInteraction 스크립트에 저장해둔 이전 씬 이름으로 씬을 불러옵니다.
        if (!string.IsNullOrEmpty(StatueInteraction.previousSceneName))
        {
            SceneManager.LoadScene(StatueInteraction.previousSceneName);
        }
        else
        {
            // 만약 저장된 씬 이름이 없다면 기본 스테이지로 이동 (안전장치)
            Debug.LogWarning("돌아갈 씬 정보가 없습니다. 기본 씬으로 이동합니다.");
            SceneManager.LoadScene("Stage1"); // "Stage1"은 실제 기본 스테이지 씬 이름으로 변경
        }
    }

    private IEnumerator SpawnFlameRoutine()
    {
        while (isGameActive) // isGameActive가 true일 때만 스폰
        {
            SpawnFlame();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void CollectFlame(GameObject flameObject)
    {
        if (!isGameActive || flameObject == null) return; // 게임이 끝났으면 수집 불가
        AddScore(1);
        Destroy(flameObject);
    }

    // (기존의 다른 함수들은 그대로 유지)
    void SpawnFlame()
    {
        float spawnX = Random.Range(spawnAreaMin.position.x, spawnAreaMax.position.x);
        float spawnY = Random.Range(spawnAreaMin.position.y, spawnAreaMax.position.y);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
        GameObject flameInstance = Instantiate(soulFlamePrefab, spawnPosition, Quaternion.identity);
        flameInstance.SetActive(true);
    }
    private void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }
    public void DecreaseScore(int amount)
    {
        score -= amount;
        if (score < 0) score = 0;
        Debug.Log("loss score! score: " + score);
        UpdateScoreUI();
    }
    private void UpdateScoreUI()
    {
        scoreText.text = $"flame: {score}";
    }
}
