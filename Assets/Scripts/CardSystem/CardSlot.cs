using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler
{
    [Header("슬롯에 표시할 카드 (수동 설정)")]
    public CombatPage cardToDisplay;
    
    [Header("생성할 카드 프리팹")]
    public GameObject cardPrefab;

    void Start()
    {
        // 게임이 시작될 때, 이 슬롯에 표시할 카드가 지정되어 있다면 카드 UI를 생성합니다.
        if (cardToDisplay != null && cardPrefab != null)
        {
            // 이 슬롯의 자식으로 카드 프리팹을 생성합니다.
            GameObject cardObject = Instantiate(cardPrefab, transform);

            // 생성된 카드 UI에 카드 데이터를 전달합니다.
            DraggableCard draggableCard = cardObject.GetComponent<DraggableCard>();
            if (draggableCard != null)
            {
                draggableCard.SetCardData(cardToDisplay);
            }
        }
    }

    // 다른 카드가 이 슬롯에 드롭되었을 때 호출됩니다.
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;

        DraggableCard draggableCard = droppedObject.GetComponent<DraggableCard>();
        if (draggableCard == null) return;

        // --- 시각적인 카드 교환 또는 이동 처리 ---
        // 이 슬롯에 카드가 이미 있다면, 원래 있던 카드를 드롭한 카드의 이전 슬롯으로 보냅니다.
        if (transform.childCount > 1) // 드롭된 카드가 임시로 자식이 되므로 1보다 큰지 체크
        {
            // 0번째 자식이 원래 있던 카드
            DraggableCard existingCard = transform.GetChild(0).GetComponent<DraggableCard>();
            if (existingCard != null)
            {
                existingCard.transform.SetParent(draggableCard.parentToReturnTo);
                existingCard.transform.localPosition = Vector3.zero;
            }
        }

        // 드롭된 카드를 이 슬롯의 자식으로 설정합니다.
        draggableCard.parentToReturnTo = this.transform;
        draggableCard.transform.SetParent(this.transform);
        draggableCard.transform.localPosition = Vector3.zero;
        
        // --- 시각적인 변경이 끝난 후, CardInventoryUI에게 데이터 저장을 요청 ---
        if (CardInventoryUI.Instance != null)
        {
            CardInventoryUI.Instance.UpdateAndSaveChanges();
        }
    }

    // 현재 이 슬롯에 있는 카드의 데이터를 반환하는 함수 (저장용)
    public CombatPage GetCurrentCardData()
    {
        if (transform.childCount > 0)
        {
            DraggableCard card = transform.GetChild(0).GetComponent<DraggableCard>();
            if (card != null)
            {
                return card.cardData;
            }
        }
        return null;
    }
}