using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

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
        // UI를 업데이트하는 대신, 이미 슬롯들이 스스로 카드를 띄웠으므로 시간을 멈추기만 합니다.
        Time.timeScale = 0f;
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
        playerCharacterStats.cardCollection = newDeck.Concat(newCollection).ToList();

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
}