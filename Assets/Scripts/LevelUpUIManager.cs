
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpUIManager : MonoBehaviour
{
    [Header("XP Bar")]
    public Slider xpSlider;
    public TextMeshProUGUI levelText;

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
