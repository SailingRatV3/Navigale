using System.Collections.Generic;
using UnityEngine;
/*
 * Add to Player Object
 * Add Inventory UI
 */
public class Inventory : MonoBehaviour
{
    public List<ItemData> items = new();

    public void AddItem(ItemData item)
    {
        items.Add(item);
        Debug.Log($"Picked up {item.itemName}");
    }

    public void UseItem(ItemData item)
    {
        item.Use(gameObject);
        items.Remove(item);
        Debug.Log($"Use {item.itemName}");
    }
    
}
