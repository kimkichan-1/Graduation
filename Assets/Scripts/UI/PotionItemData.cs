using UnityEngine;

[CreateAssetMenu(fileName = "New PotionData", menuName = "Inventory/Potion Data")]
public class PotionItemData : ItemData
{
    public int healthAmount = 20;

    public override void Use()
    {
        base.Use();
        // Find the PlayerHealth component and restore health
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(healthAmount);
        }
        // Remove the potion from the inventory
        Inventory.instance.Remove(this);
    }
}
