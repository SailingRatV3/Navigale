using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public ItemData itemData;

   public void Interact(Inventory inventory)
    {
        //Debug.Log("Item picked up!");
        inventory.AddItem(itemData);
        Destroy(gameObject);
    }
     
}
