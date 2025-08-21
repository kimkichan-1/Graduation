using UnityEngine;

[CreateAssetMenu(fileName = "New ItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName = "New Item";
    public string description = "Item Description";
    public Sprite icon = null;
}
