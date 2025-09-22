using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Button을 사용하기 위해 추가

public class CardInventoryUI : MonoBehaviour
{
    public GameObject cardInventoryPanel;
    public Transform deckContentPanel; // 덱 슬롯들의 부모 패널
    public Transform collectionContentPanel; // 소지한 카드 슬롯들의 부모 패널
    public GameObject cardPrefab; // 카드 UI 프리팹
    public Button closeButton; // 새로 추가된 닫기 버튼

    // 플레이어가 가진 모든 카드의 예시 리스트입니다.
    public List<CardData> allPlayerCards = new List<CardData>();

    void Start()
    {
        if(cardInventoryPanel != null) cardInventoryPanel.SetActive(false);

        // 닫기 버튼에 리스너를 추가합니다.
        if(closeButton != null) closeButton.onClick.AddListener(CloseInventory);

        PopulateCardCollection();
    }

    // 인벤토리 열기/닫기 토글 함수
    public void ToggleInventory()
    {
        if(cardInventoryPanel == null) return;

        bool isActive = !cardInventoryPanel.activeSelf;
        cardInventoryPanel.SetActive(isActive);

        Time.timeScale = isActive ? 0f : 1f;
    }

    // 인벤토리를 닫는 함수
    public void CloseInventory()
    {
        if(cardInventoryPanel == null) return;

        cardInventoryPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void PopulateCardCollection()
    {
        if(collectionContentPanel == null || cardPrefab == null) return;

        // 컬렉션 영역에 있는 모든 슬롯을 가져옵니다.
        CardSlot[] collectionSlots = collectionContentPanel.GetComponentsInChildren<CardSlot>();

        for (int i = 0; i < allPlayerCards.Count; i++)
        {
            // 카드보다 슬롯이 부족하면 중단합니다.
            if (i >= collectionSlots.Length)
            {
                Debug.LogWarning("카드 수보다 컬렉션 슬롯이 부족합니다.");
                break;
            }

            CardSlot slot = collectionSlots[i];

            // 슬롯의 자식으로 카드 프리팹을 생성합니다.
            GameObject cardObject = Instantiate(cardPrefab, slot.transform);
            cardObject.transform.localPosition = Vector3.zero;

            DraggableCard draggableCard = cardObject.GetComponent<DraggableCard>();
            if (draggableCard != null)
            {
                draggableCard.SetCardData(allPlayerCards[i]);
            }
        }
    }

    // 현재 덱에 있는 카드 리스트를 가져오는 함수
    public List<CardData> GetDeck()
    {
        List<CardData> deck = new List<CardData>();
        if(deckContentPanel == null) return deck;

        CardSlot[] deckSlots = deckContentPanel.GetComponentsInChildren<CardSlot>();
        foreach (CardSlot slot in deckSlots)
        {
            if (slot.transform.childCount > 0)
            {
                DraggableCard card = slot.transform.GetChild(0).GetComponent<DraggableCard>();
                if (card != null)
                {
                    deck.Add(card.cardData);
                }
            }
        }
        return deck;
    }
}
