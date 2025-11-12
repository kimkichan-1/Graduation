// ���ϸ�: BlacksmithMinigameManager.cs (���� ���� ����)
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // �� ������ ���� �߰�
using UnityEngine.Events;

public class BlacksmithMinigameManager : MonoBehaviour
{
    public static bool isGamePausedByManager = false;
    [Header("���� ���� ����")]
    [Tooltip("�̴ϰ��� ���� �� ������ ���� ���")]
    [TextArea(3, 10)]
    public string[] startDialogue;
    public static BlacksmithMinigameManager Instance { get; private set; }

    [Header("UI ���")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("�̴ϰ��� ����")]
    [SerializeField] private float gameDuration = 60f;
    [SerializeField] private GameObject soulFlamePrefab;
    [SerializeField] private float spawnInterval = 2f;

    [Header("���� ����")]
    [SerializeField] private Transform spawnAreaMin;
    [SerializeField] private Transform spawnAreaMax;

    // --- �� �κ��� �߰��ϼ��� ---
    [Header("���� UI")]
    [SerializeField] private GameObject rewardPanel; // ���� ���� �� �� �г�
    [SerializeField] private TextMeshProUGUI rewardInfoText; // ���� ������ ǥ���� �ؽ�Ʈ
    // --- ������� ---

    private float currentTime;
    private int score;
    private bool isGameActive = true; // ���� Ȱ��ȭ ���� ���� �߰�
    private bool hasGameStarted = false; // ������ ���۵Ǿ����� Ȯ���ϴ� �÷���

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        // �ڡڡ� �� �κ��� �߰��ϼ��� �ڡڡ�
        hasGameStarted = false; // ���� ���۵� �� �÷��� �ʱ�ȭ
        // --- ������� ---
        isGamePausedByManager = false;
        if (rewardPanel != null) rewardPanel.SetActive(false);
        Time.timeScale = 0f; // ���� �Ͻ�����

        // DialogueController�� ���� ���� ��ȭ�� �����մϴ�.
        if (DialogueController.Instance != null && startDialogue.Length > 0)
        {
            // ��ȭ�� ���� �Ŀ� ������ �ൿ���� 'StartMinigame' �Լ��� �����մϴ�.
            UnityEvent endAction = new UnityEvent();
            endAction.AddListener(StartMinigame);

            DialogueController.Instance.StartDialogue(startDialogue, endAction);
        }
        else
        {
            // ������ ������ ���ٸ�, �ٷ� ������ �����մϴ�.
            StartMinigame();
        }
    }
    public void StartMinigame()
    {
        // �ڡڡ� �� �κ��� �����Ǿ����ϴ� �ڡڡ�
        // ���� ������ �̹� ���۵Ǿ��ٸ�, �ƹ��͵� ���� �ʰ� �Լ��� ��� �����մϴ�.
        if (hasGameStarted)
        {
            Debug.LogWarning("StartMinigame()�� �ߺ� ȣ��Ǿ�����, ���õǾ����ϴ�.");
            return;
        }
        // ������ ���۵Ǿ��ٰ� �÷��׸� true�� �����մϴ�.
        hasGameStarted = true;
        // --- ������� ---

        Time.timeScale = 1f;
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

        EndGame(); // �ð��� �� �Ǹ� ���� ���� �Լ� ȣ��
    }

    // --- �Ʒ� �Լ����� ���� �߰��ϰų� �����մϴ� ---

    private void EndGame()
    {
        isGameActive = false;
        StopAllCoroutines();
        timerText.text = "game over!";

        // ���� ���� ��, ������ �ð��� ����ϴ�.
        Time.timeScale = 0f;
        isGamePausedByManager = true; // ������ �� �Ŵ����� ���� ����ٰ� '���'

        // �ڡڡ� �� �κ��� �����Ǿ����ϴ� �ڡڡ�
        // ���� ��� �� '��� ����'�ϵ��� ����
        CalculateAndGrantRewards();
        // �ڡڡ� ������� �ڡڡ�

        if (rewardPanel != null) rewardPanel.SetActive(true);
    }

    // �ڡڡ� �Լ� �̸��� ������ ����Ǿ����ϴ� �ڡڡ�
    private void CalculateAndGrantRewards()
    {
        string rewardMessage;
        int goldReward = 0;
        int xpReward = 0;
        int attackBonusReward = 0;

        // ����(���� �Ҳ� ����)�� ���� ���� ��ġ ���
        if (score >= 20) // ����: �Ҳ� 20�� �̻�
        {
            rewardMessage = $"The best performance!\n\nGold + 700\nExperience value +500\nAttacks +30";
            goldReward = 700;
            xpReward = 500;
            attackBonusReward = 30;
        }
        else if (score >= 10) // ����: �Ҳ� 10�� �̻�
        {
            rewardMessage = $"Great achievement!\n\nGold + 500\nExperience value +300\nAttacking +15";
            goldReward = 500;
            xpReward = 300;
            attackBonusReward = 15;
        }
        else // �� ��
        {
            rewardMessage = $"basic compensation\n\nGold +200\nExperience value +50";
            goldReward = 200;
            xpReward = 50;
        }

        // 1. UI�� ���� ������ ǥ���մϴ�.
        if (rewardInfoText != null) rewardInfoText.text = rewardMessage;

        // 2. 'Player' �±׷� �÷��̾ ã�Ƽ� PlayerStats ��ũ��Ʈ�� �����ɴϴ�.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            PlayerStats playerStats = playerObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                // 3. PlayerStats ��ũ��Ʈ�� �Լ��� ���� ȣ���Ͽ� ������ ��� �����մϴ�.
                Debug.Log("�÷��̾�� ������ ���� �����մϴ�...");
                if (goldReward > 0) playerStats.AddMoney(goldReward);
                if (xpReward > 0) playerStats.AddXp(xpReward);
                if (attackBonusReward > 0) playerStats.AddAttackPower(attackBonusReward); ; // UpgradeAttackPower�� 5�� �����Ƿ�, ������ ȣ���ϰų� ���� �޵��� ���� �ʿ�
                // ����: PlayerStats�� UpgradeAttackPower()�� 5�� �������� �����Ǿ� �ֽ��ϴ�.
                // ���� 2�� ���ʽ��� �ְ� �ʹٸ�, PlayerStats�� UpgradeAttackPower() �Լ���
                // public void UpgradeAttackPower(int amount) { bonusAttackPower += amount; ... } �� ���� �����ؾ� �մϴ�.
                // ������ ���� �Լ��� �״�� Ȱ���ϰڽ��ϴ�.
            }
            else
            {
                Debug.LogError("�÷��̾� ������Ʈ���� PlayerStats ��ũ��Ʈ�� ã�� �� �����ϴ�!");
            }
        }
        else
        {
            Debug.LogError("'Player' �±׸� ���� �÷��̾� ������Ʈ�� ������ ã�� �� �����ϴ�!");
        }
    }

    // '���ư���' ��ư�� ������ �Լ�
    public void ReturnToPreviousScene()
    {
        Time.timeScale = 1f;
        isGamePausedByManager = false; // ������ �ٽ� ���۵ǹǷ� '���'�� ����
        // StatueInteraction ��ũ��Ʈ�� �����ص� ���� �� �̸����� ���� �ҷ��ɴϴ�.
        if (!string.IsNullOrEmpty(StatueInteraction.previousSceneName))
        {
            SceneManager.LoadScene(StatueInteraction.previousSceneName);
        }
        else
        {
            // ���� ����� �� �̸��� ���ٸ� �⺻ ���������� �̵� (������ġ)
            Debug.LogWarning("���ư� �� ������ �����ϴ�. �⺻ ������ �̵��մϴ�.");
            SceneManager.LoadScene("Stage1"); // "Stage1"�� ���� �⺻ �������� �� �̸����� ����
        }
    }

    private IEnumerator SpawnFlameRoutine()
    {
        while (isGameActive) // isGameActive�� true�� ���� ����
        {
            SpawnFlame();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void CollectFlame(GameObject flameObject)
    {
        if (!isGameActive || flameObject == null) return; // ������ �������� ���� �Ұ�
        AddScore(1);
        Destroy(flameObject);
    }

    // (������ �ٸ� �Լ����� �״�� ����)
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

    /// <summary>
    /// static 변수 초기화 (게임 오버 또는 메인 메뉴 복귀 시 호출)
    /// </summary>
    public static void ResetStaticVariables()
    {
        isGamePausedByManager = false;
        Debug.Log("BlacksmithMinigameManager: isGamePausedByManager 초기화됨");
    }
}
