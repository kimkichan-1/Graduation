using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int level = 1;
    public int currentXp = 0;
    public int xpToNextLevel = 100;

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

    void Start()
    {
        if (uiManager != null)
        {
            uiManager.UpdateLevelText(level);
            uiManager.UpdateXpBar(currentXp, xpToNextLevel);
        }
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

        if (uiManager != null)
        {
            uiManager.UpdateLevelText(level);
            uiManager.UpdateXpBar(currentXp, xpToNextLevel);
            uiManager.ShowLevelUpPanel(true);
        }
        Time.timeScale = 0f;
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

        Time.timeScale = 1f; // 게임 재개
    }
}
