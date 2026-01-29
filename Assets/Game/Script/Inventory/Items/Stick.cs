using UnityEngine;
[CreateAssetMenu(menuName = "Items/Stick Item")]
public class Stick : ItemData
{
    
    public override void Use(GameObject player)
    {
        Debug.Log($"Using Stick");
    }
}
