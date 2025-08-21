using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData item; // The specific item data for this pickup

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the trigger is the player
        if (other.CompareTag("Player"))
        {
            PickUp();
        }
    }

    void PickUp()
    {
        Debug.Log("Picking up " + item.itemName);
        // Add to inventory
        bool wasPickedUp = Inventory.instance.Add(item);

        // If successfully picked up, destroy the item from the scene
        if (wasPickedUp)
        {
            Destroy(gameObject);
        }
    }
}
