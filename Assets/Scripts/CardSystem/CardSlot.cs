using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        if (droppedObject == null) return;

        DraggableCard draggableCard = droppedObject.GetComponent<DraggableCard>();
        if (draggableCard == null) return;

        // 이 슬롯에 카드가 이미 있는지 확인합니다.
        if (transform.childCount > 0)
        {
            // 현재 슬롯에 있는 카드를 가져옵니다.
            DraggableCard existingCard = transform.GetChild(0).GetComponent<DraggableCard>();
            if (existingCard != null)
            {
                // 원래 있던 카드를 드롭한 카드의 이전 슬롯으로 보냅니다. (카드 교환)
                existingCard.transform.SetParent(draggableCard.parentToReturnTo);
                existingCard.transform.localPosition = Vector3.zero;
            }
        }

        // 드롭된 카드를 이 슬롯으로 이동시킵니다.
        draggableCard.parentToReturnTo = this.transform;
        draggableCard.transform.SetParent(this.transform);
        draggableCard.transform.localPosition = Vector3.zero; // 슬롯의 중앙에 위치시킵니다.
    }
}
