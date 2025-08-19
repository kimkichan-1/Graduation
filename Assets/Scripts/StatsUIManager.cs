
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

        void Start()
        {
            // 게임 시작 시 초기 값으로 설정
            UpdateStatsUI(null);
        }

        public void UpdateStatsUI(WeaponStats stats)
        {
            if (stats != null)
            {
                // 무기 스탯이 있으면 해당 값으로 UI 업데이트
                weaponNameText.text = stats.weaponName;
                attackPowerText.text = stats.attackPower.ToString();
                defenseText.text = stats.defense.ToString();
                bonusHealthText.text = stats.bonusHealth.ToString();
                moveSpeedText.text = stats.moveSpeed.ToString("F1");
                dashForceText.text = stats.dashForce.ToString("F1");
                dashDurationText.text = stats.dashDuration.ToString("F2");
                dashCooldownText.text = stats.dashCooldown.ToString("F2");
            }
            else
            {
                // 무기 스탯이 없으면 (초기 상태) 기본 값으로 설정
                weaponNameText.text = "없음";
                attackPowerText.text = "0";
                defenseText.text = "0";
                bonusHealthText.text = "0";
                moveSpeedText.text = "0.0";
                dashForceText.text = "0.0";
                dashDurationText.text = "0.00";
                dashCooldownText.text = "0.00";
            }
        }
    }
    