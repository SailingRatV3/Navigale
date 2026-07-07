using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
   [Header("Interaction References")]
   
   public float interactDistance = 2f;
   public LayerMask interactLayer;
   private Inventory inventory;

   private IInteractable currentInteractable;

   public InputActionReference interactAction;
   
//-----------------------------------------------------------------------------//
   [Header("Health References")]
   public float playerHealth, playerMaxHealth;
   public static event Action OnPlayerDeath;
   public static event Action OnPlayerHit;
   
   public void Start()
   {
     // playerHealth = playerMaxHealth;
   }
   
   // Player Take Damage
   public void TakeDamage(float amount)
   {
      playerHealth -= amount;
      OnPlayerHit?.Invoke();
      
      // If player has no health
      if (playerHealth <= 0)
      {
         playerHealth = 0;
         Debug.Log("Player is dead");
         OnPlayerDeath?.Invoke();
      }
   }

   void Awake()
   {
      inventory = GetComponent<Inventory>();
   }

   void Update()
   {
      DetectInteractable();
   }

   void OnEnable()
   {
      interactAction.action.performed += OnInteractPerformed;
      interactAction.action.Enable();
   }

   void OnDisable()
   {
      interactAction.action.performed -= OnInteractPerformed;
      interactAction.action.Disable();
   }

   void DetectInteractable()
   {
      
         Vector3 origin = transform.position + Vector3.down * 1.2f;
         float radius = 1f;

         Collider[] hits = Physics.OverlapSphere(origin, radius, interactLayer);
        //Debug.Log("Hits found: " + hits.Length);

         if (hits.Length > 0)
         {
            currentInteractable = hits[0].GetComponentInParent<IInteractable>();
           // if (currentInteractable != null)
            // Debug.Log("Found interactable: " + hits[0].name);
         }
         else
         {
            currentInteractable = null;
         }

         DebugDrawSphere(origin, radius, Color.green);
      
   }
   
   // Debug sphere for detection
   void DebugDrawSphere(Vector3 position, float radius, Color color)
   {
      // Draw a sphere with 12 lines
      int segments = 12;
      for (int i = 0; i < segments; i++)
      {
         float theta1 = (i / (float)segments) * Mathf.PI * 2;
         float theta2 = ((i + 1) / (float)segments) * Mathf.PI * 2;
         Vector3 p1 = position + new Vector3(Mathf.Cos(theta1), 0, Mathf.Sin(theta1)) * radius;
         Vector3 p2 = position + new Vector3(Mathf.Cos(theta2), 0, Mathf.Sin(theta2)) * radius;
         Debug.DrawLine(p1, p2, color);
      }
   }

   private void OnInteractPerformed(InputAction.CallbackContext context)
   {
      if (currentInteractable != null)
      {
        // Debug.Log("Item picked up via raycast!");
         currentInteractable.Interact(inventory);
         currentInteractable = null; // reset to avoid double pickup
      }
   }
}
