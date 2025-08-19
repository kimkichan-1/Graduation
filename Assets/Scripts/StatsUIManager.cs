using UnityEngine;
using TMPro;

public class StatsUIManager : MonoBehaviour
{
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI attackPowerText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI bonusHealthText;
    public TextMeshProUGUI moveSpeedText;
    public TextMeshProUGUI dashForceText;
    public TextMeshProUGUI dashDurationText;
    public TextMeshProUGUI dashCooldownText;

    private string statFormat = "{0} <color=green>(+{1})</color>";

    public void UpdateStatsUI(WeaponStats weapon, PlayerStats playerStats, PlayerController playerController, PlayerHealth playerHealth)
    {
        if (weapon != null)
        {
            // 무기 장착 시
            weaponNameText.text = weapon.weaponName;
            attackPowerText.text = string.Format(statFormat, weapon.attackPower, playerStats.bonusAttackPower);
            defenseText.text = string.Format(statFormat, weapon.defense, playerHealth.defense);
            bonusHealthText.text = string.Format(statFormat, weapon.bonusHealth, playerHealth.baseMaxHealth - 100); // 100을 기본 체력으로 가정
            moveSpeedText.text = string.Format(statFormat, weapon.moveSpeed, playerStats.bonusMoveSpeed);
            dashForceText.text = weapon.dashForce.ToString("F1");
            dashDurationText.text = weapon.dashDuration.ToString("F2");
            dashCooldownText.text = weapon.dashCooldown.ToString("F2");
        }
        else
        {
            // 무기 미장착 시
            weaponNameText.text = "맨손";
            attackPowerText.text = string.Format(statFormat, 0, playerStats.bonusAttackPower);
            defenseText.text = playerHealth.defense.ToString();
            bonusHealthText.text = (playerHealth.baseMaxHealth - 100).ToString();
            moveSpeedText.text = playerController.moveSpeed.ToString("F1");
            dashForceText.text = playerController.dashForce.ToString("F1");
            dashDurationText.text = "0.00";
            dashCooldownText.text = "0.00";
        }
    }
}
