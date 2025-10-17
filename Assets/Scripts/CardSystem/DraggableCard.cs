using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CombatPage cardData;
    public Image artworkImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText; // ★★★ 코스트 텍스트 참조 추가 ★★★

    [HideInInspector]
    public Transform parentToReturnTo = null;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void SetCardData(CombatPage data)
    {
        cardData = data;
        if (data == null) return;
        
        // ★★★ UI 텍스트 설정 로직 수정 ★★★
        if (nameText != null)
        {
            nameText.text = cardData.pageName;
        }

        if (costText != null)
        {
            costText.text = cardData.lightCost.ToString();
        }

        if (descriptionText != null)
        {
            string diceInfo = "";
            foreach (var dice in cardData.diceList)
            {
                diceInfo += $"[{dice.type.ToString()} {dice.minValue}-{dice.maxValue}] ";
            }
            descriptionText.text = diceInfo;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.root);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(parentToReturnTo);
        this.transform.localPosition = Vector3.zero;
        canvasGroup.blocksRaycasts = true;
    }
}