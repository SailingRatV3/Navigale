using UnityEngine;
/// <summary>
/// Attach to objects that if player enter its collison it will take dmg
/// </summary>
public class DmgCollision : MonoBehaviour
{
    [SerializeField] float damage = 1;
    
    private void OnTriggerEnter(Collider collision)
    {
         if (collision.gameObject.CompareTag("Player"))
         {
             collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
         }
    }
    
   
}
