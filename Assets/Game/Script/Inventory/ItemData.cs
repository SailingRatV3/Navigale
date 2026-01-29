using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public bool stackable = true;

    public abstract void Use(GameObject player);

    // Add Throw Item, Drop Item, Item Effects 
    // Max Item Storage
    // Save and Load System + Inventory
    

}
