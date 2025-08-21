using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Transform slotsParent; // The parent object of all the slots (e.g., SlotContainer)

    private bool isInventoryOpen = false;
    private InventorySlot[] slots;

    void Start()
    {
        // Subscribe to the inventory changed callback
        Inventory.instance.onInventoryChangedCallback += UpdateUI;

        // Get all slot components
        slots = slotsParent.GetComponentsInChildren<InventorySlot>();

        // Initially, the inventory is closed.
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

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
        if(isInventoryOpen) {
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
