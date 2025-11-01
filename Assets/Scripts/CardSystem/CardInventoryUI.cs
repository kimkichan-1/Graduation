using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardInventoryUI : MonoBehaviour
{
    public static CardInventoryUI Instance;

    [Header("UI Panels")]
    public GameObject cardInventoryPanel;
    public Transform deckContentPanel;
    public Transform collectionContentPanel;
    
    [Header("Settings")]
    public Button closeButton;

    private CharacterStats playerCharacterStats;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if(cardInventoryPanel != null) cardInventoryPanel.SetActive(false);
        if(closeButton != null) closeButton.onClick.AddListener(CloseInventory);

        if (PlayerController.Instance != null)
        {
            playerCharacterStats = PlayerController.Instance.GetComponent<CharacterStats>();
        }
    }
    
    public void OpenCardInventory()
    {
        if (cardInventoryPanel != null)
        {
            cardInventoryPanel.SetActive(true);
        }

        // ▼▼▼ [핵심] UI를 여는 시점에 데이터를 로드하는 함수 호출 ▼▼▼
        UpdateUIFromCharacterStats(); 

        Time.timeScale = 0f;
        GameObject pearlGroup = GameObject.Find("PearlUI_Group");
        if (pearlGroup != null)
        {
            // 찾았다면 비활성화합니다.
            pearlGroup.SetActive(false);
            Debug.Log("PearlUI_Group 숨김 처리.");
        }
        else
        {
            Debug.LogWarning("씬에서 'PearlUI_Group' 오브젝트를 찾을 수 없습니다."); // 못 찾으면 경고 메시지
        }
    }

    public void CloseInventory()
    {
        if (cardInventoryPanel != null)
        {
            cardInventoryPanel.SetActive(false);
        }
        // 닫기 전에 최종 상태를 저장합니다.
        UpdateAndSaveChanges();
        Time.timeScale = 1f;
        if (SceneManager.GetActiveScene().name == "Stage3")
        {
            // 씬에서 이름이 "PearlUI_Group"인 게임 오브젝트를 찾습니다.
            GameObject pearlGroup = GameObject.Find("PearlUI_Group");
            if (pearlGroup != null)
            {
                // 찾았다면 다시 활성화합니다.
                pearlGroup.SetActive(true);
                Debug.Log("PearlUI_Group 다시 활성화 (Stage3).");
            }
            // Stage3인데도 못 찾으면 경고 (오브젝트 이름이 다르거나 없을 경우)
            else
            {
                Debug.LogWarning("Stage3이지만 'PearlUI_Group' 오브젝트를 찾을 수 없습니다.");
            }
        }
    }

    // 현재 슬롯들의 상태를 읽어 CharacterStats에 저장하는 함수
    public void UpdateAndSaveChanges()
    {
        if (playerCharacterStats == null) return;

        // 각 패널의 슬롯들로부터 현재 카드 데이터를 읽어옵니다.
        List<CombatPage> newDeck = GetCardsFromPanel(deckContentPanel);
        List<CombatPage> newCollection = GetCardsFromPanel(collectionContentPanel);

        // 읽어온 정보로 CharacterStats의 데이터를 업데이트합니다.
        playerCharacterStats.deck = newDeck;

        // [수정] 전체 컬렉션 = 덱 + 컬렉션 패널 카드들 (중복 제거)
        List<CombatPage> totalCollection = new List<CombatPage>(newDeck);
        foreach (var card in newCollection)
        {
            if (!totalCollection.Contains(card))
            {
                totalCollection.Add(card);
            }
        }
        playerCharacterStats.cardCollection = totalCollection;

        playerCharacterStats.SortDeckByCost();
        Debug.Log("덱이 실시간으로 저장되었습니다!");
    }

    // 지정된 패널의 모든 슬롯들로부터 현재 카드 데이터를 읽어오는 함수
    List<CombatPage> GetCardsFromPanel(Transform panel)
    {
        List<CombatPage> cards = new List<CombatPage>();
        if(panel == null) return cards;

        CardSlot[] slots = panel.GetComponentsInChildren<CardSlot>();
        foreach (CardSlot slot in slots)
        {
            CombatPage cardData = slot.GetCurrentCardData();
            if (cardData != null)
            {
                cards.Add(cardData);
            }
        }
        return cards;
    }

    // ▼▼▼ [핵심] 이 함수가 새로 추가되었습니다 ▼▼▼
    /// <summary>
    /// CharacterStats의 데이터를 기반으로 덱/컬렉션 UI를 다시 그립니다.
    /// (각 슬롯의 SetCard 함수를 호출)
    /// </summary>
    public void UpdateUIFromCharacterStats()
    {
        if (playerCharacterStats == null)
        {
            Debug.LogError("PlayerCharacterStats가 없습니다!");
            return;
        }

        // 1. 모든 덱 슬롯과 컬렉션 슬롯을 가져옵니다.
        CardSlot[] deckSlots = deckContentPanel.GetComponentsInChildren<CardSlot>();
        CardSlot[] collectionSlots = collectionContentPanel.GetComponentsInChildren<CardSlot>();

        // 2. [덱 패널] playerCharacterStats.deck 리스트를 기반으로 덱 슬롯을 채웁니다.
        List<CombatPage> deck = playerCharacterStats.deck;
        for (int i = 0; i < deckSlots.Length; i++)
        {
            if (i < deck.Count)
            {
                // 덱 카드 리스트에 카드가 있으면 슬롯에 설정
                deckSlots[i].SetCard(deck[i]);
            }
            else
            {
                // 덱 카드보다 슬롯이 많으면 남는 슬롯은 비움
                deckSlots[i].SetCard(null); 
            }
        }

        // 3. [컬렉션 패널] 덱에 포함되지 않은 카드들만 추려냅니다.
        List<CombatPage> cardsInDeck = new List<CombatPage>(playerCharacterStats.deck);
        List<CombatPage> cardsForCollectionPanel = new List<CombatPage>();

        foreach (CombatPage cardInCollection in playerCharacterStats.cardCollection)
        {
            // 덱 리스트에서 이 카드(와 동일한 참조)를 찾습니다.
            CombatPage match = cardsInDeck.FirstOrDefault(c => c == cardInCollection);
            
            if (match != null)
            {
                // 덱에 있는 카드입니다. 덱 리스트에서 하나 제거합니다 (중복 처리를 위해).
                cardsInDeck.Remove(match);
            }
            else
            {
                // 덱에 없는 카드(매칭되는 인스턴스가 없는)이므로 컬렉션 패널에 추가합니다.
                cardsForCollectionPanel.Add(cardInCollection);
            }
        }

        // 4. [컬렉션 패널] 추려낸 카드들을 컬렉션 슬롯에 채웁니다.
        for (int i = 0; i < collectionSlots.Length; i++)
        {
            if (i < cardsForCollectionPanel.Count)
            {
                // 컬렉션 카드 리스트에 카드가 있으면 슬롯에 설정
                collectionSlots[i].SetCard(cardsForCollectionPanel[i]);
            }
            else
            {
                // 컬렉션 카드보다 슬롯이 많으면 남는 슬롯은 비움
                collectionSlots[i].SetCard(null); 
            }
        }
    }
}