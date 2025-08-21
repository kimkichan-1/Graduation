using UnityEngine;

[CreateAssetMenu(fileName = "New ItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName = "New Item";
    public string description = "Item Description";
    public Sprite icon = null;

    public virtual void Use()
    {
        // Base implementation: by default, nothing happens.
        Debug.Log("Using " + itemName);
    }
}
