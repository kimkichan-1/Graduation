using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Transform slotsParent;

    private bool isInventoryOpen = false;
    private InventorySlot[] slots;

    void Start()
    {
        Inventory.instance.onInventoryChangedCallback += UpdateUI;
        slots = slotsParent.GetComponentsInChildren<InventorySlot>();
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    // ★★★ 이 Update 함수를 다시 추가하거나 주석 해제하세요 ★★★
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
        if(isInventoryOpen) 
        {
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < Inventory.instance.items.Count)
            {
                slots[i].AddItem(Inventory.instance.items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}