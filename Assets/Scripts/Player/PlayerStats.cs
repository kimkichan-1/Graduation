using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    // 미뤄둔 레벨업이 있는지 PlayerStats 스스로 기억하는 변수
    private static bool isLevelUpPending = false;
    public delegate void OnMoneyChanged(int currentMoney);
    public static event OnMoneyChanged onMoneyChanged;

    public int level = 1;
    public int currentXp = 0;
    public int xpToNextLevel = 100;
    public int currentMoney = 0;

    [Header("Permanent Stat Bonuses")]
    public int bonusAttackPower = 0;
    public float bonusMoveSpeed = 0f;

    // 스탯 강화를 위해 다른 컴포넌트 참조
    private PlayerController playerController;
    private PlayerHealth playerHealth;

    // UI 관리자 참조
    public LevelUpUIManager uiManager;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerHealth = GetComponent<PlayerHealth>();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드되었을 때, 만약 보여줘야 할 레벨업 창이 예약되어 있다면
        if (isLevelUpPending)
        {
            // "Stage1" 같은 메인 스테이지로 돌아왔을 때만 UI를 띄웁니다.
            // (미니게임 씬에서는 띄우지 않도록 안전장치)
            if (scene.name != "AncientBlacksmith")
            {
                ShowPendingLevelUpPanel();
            }
        }
    }

    void Start()
    {
        // 세이브 로드 중이면 Start에서 초기화하지 않음 (GameManager가 데이터 적용 후 UI 업데이트)
        if (GameManager.Instance != null && GameManager.Instance.isLoadingGame)
        {
            Debug.Log("세이브 로드 중이므로 PlayerStats 초기화를 건너뜁니다.");
            return;
        }

        if (uiManager != null)
        {
            uiManager.UpdateLevelText(level);
            uiManager.UpdateXpBar(currentXp, xpToNextLevel);
        }
        // Notify UI about initial money
        onMoneyChanged?.Invoke(currentMoney);
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        onMoneyChanged?.Invoke(currentMoney);
        Debug.Log($"{amount} gold acquired. Total gold: {currentMoney}");
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            onMoneyChanged?.Invoke(currentMoney);
            return true;
        }
        else
        {
            Debug.Log("Not enough gold.");
            return false;
        }
    }

    /// <summary>
    /// Money UI 업데이트 (외부에서 호출용)
    /// </summary>
    public void NotifyMoneyChanged()
    {
        onMoneyChanged?.Invoke(currentMoney);
    }

    public void AddXp(int amount)
    {
        currentXp += amount;
        if (uiManager != null)
        {
            uiManager.UpdateXpBar(currentXp, xpToNextLevel);
        }
        CheckForLevelUp();
    }

    private void CheckForLevelUp()
    {
        if (currentXp >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentXp -= xpToNextLevel;
        level++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);

        // --- ★★★ 이 부분이 수정되었습니다 ★★★ ---
        // 현재 씬이 '고대 대장간' 씬인지 확인합니다. (씬 이름을 실제 파일 이름으로 정확히 맞춰주세요)
        bool inMinigame = SceneManager.GetActiveScene().name == "AncientBlacksmith";

        if (inMinigame)
        {
            // 미니게임 중이라면: 레벨업 예약을 하고 UI는 띄우지 않음
            isLevelUpPending = true;
            Debug.Log("미니게임 중 레벨업! UI 표시는 나중에 합니다.");
        }
        else
        {
            // 일반 상황이라면: 즉시 레벨업 UI를 띄우고 시간을 멈춤
            ShowPendingLevelUpPanel();
        }
        // --- 여기까지 ---
    }
    public void ShowPendingLevelUpPanel()
    {
        isLevelUpPending = false; // 예약 플래그를 해제

        if (uiManager != null)
        {
            uiManager.UpdateLevelText(level);
            uiManager.UpdateXpBar(currentXp, xpToNextLevel);
            uiManager.ShowLevelUpPanel(true);
        }
        Time.timeScale = 0f;
    }
    // 미니게임 등 외부에서 보상으로 공격력을 지급할 때 사용하는 함수
    public void AddAttackPower(int amount)
    {
        bonusAttackPower += amount;
        Debug.Log($"보너스 공격력이 {amount}만큼 증가했습니다! 현재 보너스: {bonusAttackPower}");

        // PlayerController가 있다면 스탯을 즉시 재계산하도록 요청
        if (playerController != null)
        {
            playerController.RecalculateStats();
            playerController.UpdateAllStatsUI();
        }
    }

    // --- 스탯 강화 메서드 (UI 버튼에서 호출) ---
    public void UpgradeAttackPower()
    {
        bonusAttackPower += 5;
        Debug.Log("영구 공격력 증가! 현재 보너스: " + bonusAttackPower);
        FinishUpgrade();
    }

    public void UpgradeDefense()
    {
        playerHealth.defense += 2;
        Debug.Log("방어력 증가! 현재 방어력: " + playerHealth.defense);
        FinishUpgrade();
    }

    public void UpgradeBonusHealth()
    {
        playerHealth.AddPermanentHealth(20);
        Debug.Log("최대 체력 증가!");
        FinishUpgrade();
    }

    public void UpgradeMoveSpeed()
    {
        bonusMoveSpeed += 0.5f;
        Debug.Log("이동 속도 보너스 증가! 현재 보너스: " + bonusMoveSpeed);
        FinishUpgrade();
    }

    private void FinishUpgrade()
    {
        if (uiManager != null)
        {
            uiManager.ShowLevelUpPanel(false);
        }

        if (playerController != null)
        {
            playerController.RecalculateStats(); // 플레이어 최종 스탯 재계산 요청
            playerController.UpdateAllStatsUI();
        }
        if (!BlacksmithMinigameManager.isGamePausedByManager)
        {
            Time.timeScale = 1f; // 게임 재개
        }
    }
}
