
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpUIManager : MonoBehaviour
{
    [Header("XP Bar")]
    public Slider xpSlider;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText; // 경험치 텍스트 (현재경험치/전체경험치)

    [Header("Level Up Panel")]
    public GameObject levelUpPanel;

    void Start()
    {
        // 시작할 때 레벨업 패널은 숨김
        if(levelUpPanel != null) levelUpPanel.SetActive(false);
    }

    public void UpdateXpBar(float currentXp, float requiredXp)
    {
        if (xpSlider != null)
        {
            xpSlider.value = currentXp / requiredXp;
        }

        // 경험치 텍스트 업데이트
        if (xpText != null)
        {
            xpText.text = $"{(int)currentXp} / {(int)requiredXp}";
        }
    }

    public void UpdateLevelText(int level)
    {
        if (levelText != null)
        {
            levelText.text = "Lv. " + level;
        }
    }

    public void ShowLevelUpPanel(bool show)
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(show);
        }
    }
}
