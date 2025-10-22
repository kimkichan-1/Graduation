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

        // UI를 업데이트하는 대신, 이미 슬롯들이 스스로 카드를 띄웠으므로 시간을 멈추기만 합니다.
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

                // (선택사항) PearlDisplayUI 스크립트가 있다면 UI 강제 업데이트
                // PearlDisplayUI displayScript = pearlGroup.GetComponentInChildren<PearlDisplayUI>(); // 자식에서 찾거나 직접 찾기
                // if(displayScript != null) displayScript.UpdatePearlUI();
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