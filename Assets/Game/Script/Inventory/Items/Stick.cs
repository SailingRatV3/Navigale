using UnityEngine;
[CreateAssetMenu(menuName = "Items/Stick Item")]
public class Stick : ItemData
{
    public override void Equip(GameObject player)
    {
        Transform hand = player.transform.Find("Hand");
        
        // remove old item
        foreach (Transform child in hand)
        {
            Destroy(child.gameObject);
        }
        // Equip new item
       GameObject currentItem = Instantiate(itemPrefab, hand);
       currentItem.transform.localPosition = Vector3.zero;
       currentItem.transform.localRotation = Quaternion.identity;
    }

    public override void Use(GameObject player)
    {
        Debug.Log($"Using Stick");
    }
}
