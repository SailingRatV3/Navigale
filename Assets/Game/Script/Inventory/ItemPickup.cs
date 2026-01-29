using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;

    void OnTriggerEnter(Collider other)
    {
        // Change it to Player using the interact button/key to pickup object
        
        if(!other.CompareTag("Player")) return;
        
        Inventory inventory = other.GetComponent<Inventory>();
        if (inventory == null) return;
        
        inventory.AddItem(itemData);
        Destroy(gameObject);
    }
}
