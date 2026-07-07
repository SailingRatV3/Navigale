using UnityEngine;
/*
 * This Class helps stack items in the inventory if it's the same collected item
 * Answers: How many items the player have
 */
[System.Serializable]
public class InventoryItem
{
    public ItemData itemData;
    public int quantity;

    public InventoryItem(ItemData itemData, int quantity = 1)
    {
        this.itemData = itemData;
        this.quantity = quantity;
    }
}
