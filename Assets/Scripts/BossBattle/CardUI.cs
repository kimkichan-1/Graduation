using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("UI 요소")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI diceDescriptionText;
    public Image cardBackgroundImage;
    public GameObject cooldownOverlay;
    public TextMeshProUGUI cooldownText;
    public GameObject selectionHighlight; // 선택 하이라이트

    [Header("상태 색상")]
    public Color affordableColor = Color.white;
    public Color unaffordableColor = new Color(0.7f, 0.7f, 0.7f, 0.8f);

    public CombatPage assignedPage { get; private set; }
    private BattleController battleController;
    private Button cardButton;

    public void Setup(CombatPage page, BattleController controller, CharacterStats ownerStats, bool isRevealed)
    {
        assignedPage = page;
        battleController = controller;
        cardButton = GetComponent<Button>();

        // 카드 정보 표시
        if (isRevealed)
        {
            nameText.text = assignedPage.pageName;
            costText.text = assignedPage.lightCost.ToString();
            string diceInfo = "";
            foreach (var dice in assignedPage.diceList)
            {
                diceInfo += $"[{dice.type.ToString()} {dice.minValue}-{dice.maxValue}] ";
            }
            diceDescriptionText.text = diceInfo;
        }
        else
        {
            nameText.text = "???";
            costText.text = "?";
            diceDescriptionText.text = "[ ??? ]";
        }

        // 초기 상태 설정
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(false);
        }
        UpdateState(ownerStats); // UpdateState는 이제 인자 1개만 받습니다.
        
        cardButton.onClick.AddListener(OnCardClicked);
    }

    // 카드 상태(사용 가능, 빛 부족, 쿨다운)에 따라 UI를 갱신하는 함수
    public void UpdateState(CharacterStats ownerStats)
    {
        bool isOnCooldown = !ownerStats.IsCardUsable(assignedPage);
        bool canAfford = ownerStats.currentLight >= assignedPage.lightCost;

        // 보스 덱을 보거나 쿨다운 중일 때는 클릭 불가
        if (battleController.IsViewingBossDeck() || isOnCooldown)
        {
            cardButton.interactable = false;
        }
        else
        {
            // 빛이 충분해야만 클릭 가능
            cardButton.interactable = canAfford;
        }

        // 시각적 효과 처리
        if (isOnCooldown)
        {
            cooldownOverlay.SetActive(true);
            cooldownText.text = ownerStats.cardCooldowns[assignedPage].ToString();
            cardBackgroundImage.color = unaffordableColor;
        }
        else
        {
            cooldownOverlay.SetActive(false);
            cardBackgroundImage.color = canAfford ? affordableColor : unaffordableColor;
        }
    }
    
    // 선택 하이라이트를 켜고 끄는 함수
    public void SetSelected(bool isSelected)
    {
        if (selectionHighlight != null)
        {
            selectionHighlight.SetActive(isSelected);
        }
    }

    private void OnCardClicked()
    {
        if (battleController.IsViewingBossDeck()) return;
        battleController.SelectCardForAction(this);
    }
}