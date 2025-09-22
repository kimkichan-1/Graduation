using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CardData cardData;
    public Image artworkImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

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

    public void SetCardData(CardData data)
    {
        cardData = data;
        if (data == null) return;
        nameText.text = cardData.cardName;
        descriptionText.text = cardData.description;
        if (artworkImage != null)
        {
            artworkImage.sprite = cardData.artwork;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.root); // 최상위 캔버스로 이동하여 다른 UI 위에 보이게 합니다.
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 유효한 슬롯에 드롭되지 않았다면, 원래 부모(슬롯)로 돌아갑니다.
        if (this.transform.parent == this.transform.root)
        {
            this.transform.SetParent(parentToReturnTo);
            this.transform.localPosition = Vector3.zero;
        }
        canvasGroup.blocksRaycasts = true;
    }
}
