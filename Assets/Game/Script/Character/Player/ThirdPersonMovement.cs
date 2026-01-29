using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("References")]
    

    
   public CharacterController controller;
   public Transform cam;
   
   [Header("Movement")]
   public InputActionReference moveAction;
   public InputActionReference runAction;
   public float speed = 6f;
   public float turnSmoothTime = 0.1f;
   
   float turnSmoothVelocity;

   public float runMutliplier = 1.8f;
   bool isRunning;

   [Header("Jump Settings")]
   public InputActionReference jumpAction;
   public float gravity = 25f;
   public float jumpHeight = 3f;
   public float jumpSpeed = 1f;
   float verticalVelocity;
   bool isGrounded;
   void OnEnable()
   {
       moveAction.action.Enable();
       jumpAction.action.Enable();
       runAction.action.Enable();
       
   }

   void OnDisable()
   {
       moveAction.action.Disable();
      jumpAction.action.Disable();
      runAction.action.Disable();
   }
   
   void Update()
   {
       // Jumping - IsGrounded
       isGrounded = controller.isGrounded;

       if (isGrounded && verticalVelocity < 0)
       {
           verticalVelocity = -2f;
       }
       
       
       // Movement
       Vector2 input = moveAction.action.ReadValue<Vector2>();
       Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;
       
       // Run
       isRunning = runAction.action.IsPressed();
       float currentSpeed = isRunning ? speed * runMutliplier : speed;

    if (direction.magnitude >= 0.1f)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
       
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        controller.Move(moveDir.normalized * (currentSpeed * Time.deltaTime));
    }

    if (jumpAction.action.triggered && isGrounded)
    {
        verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
    }
    
    verticalVelocity -= gravity * Time.deltaTime;
    
    controller.Move(Vector3.up * (verticalVelocity * Time.deltaTime));
    
   }

   

}
