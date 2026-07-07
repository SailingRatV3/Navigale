using System.Collections.Generic;
using UnityEngine;
/*
 * Add to Player Object
 * Stack Items + List Items
 * Add Inventory UI
 Add Description
 */
public class Inventory : MonoBehaviour
{
    public List<InventoryItem> items = new();

    public InventoryManager inventoryManager; // connect Inventory to Inventory Manager
    public void AddItem(ItemData item)
    {
       InventoryItem existingItem = null;
        if (item.stackable)
        {
            // Check if an Item exists in the Inventory 
             existingItem = items.Find(x => x.itemData == item);
            if (existingItem != null)
            {
                existingItem.quantity++;
                Debug.Log($"Added {item.itemName}. Total: {existingItem.quantity}");
               inventoryManager.RefreshUI(items);
                return;
            }
        }
        
        // Add New Entry
        items.Add(new InventoryItem(item, 1));
        Debug.Log($"Picked up {item.itemName}");
        inventoryManager.RefreshUI(items);
    }

    public void UseItem(ItemData item)
    {
        InventoryItem inventoryItem = items.Find(x => x.itemData == item);
        if (inventoryItem != null)
        {
            item.Use(gameObject);

            if (inventoryItem.itemData.stackable && inventoryItem.quantity > 1)
            {
                inventoryItem.quantity--;
                Debug.Log($"Used {item.itemName}. Remaining: {inventoryItem.quantity}");
            }
            else
            {
                items.Remove(inventoryItem);
                Debug.Log($"Used {item.itemName}. Removed from inventory.");
            }
            inventoryManager.RefreshUI(items);
        }
        else
        {
            Debug.LogWarning($"Tried to use {item.itemName}, but it was not in the inventory.");
        }
    }
    public void EquipItem(ItemData item)
    {
        InventoryItem inventoryItem = items.Find(x => x.itemData == item);
        if (inventoryItem != null)
        {
            item.Equip(gameObject);

            if (inventoryItem.itemData.stackable && inventoryItem.quantity > 1)
            {
                inventoryItem.quantity--;
                Debug.Log($"Used {item.itemName}. Remaining: {inventoryItem.quantity}");
            }
            else
            {
                items.Remove(inventoryItem);
                Debug.Log($"Used {item.itemName}. Removed from inventory.");
            }
            inventoryManager.RefreshUI(items);
        }
        else
        {
            Debug.LogWarning($"Tried to use {item.itemName}, but it was not in the inventory.");
        }
    }
}
    

