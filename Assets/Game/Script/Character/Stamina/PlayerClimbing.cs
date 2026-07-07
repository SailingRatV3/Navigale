using System;
using Mono.Cecil.Cil;
using Unity.Cinemachine;
using UnityEngine;
using Unity.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal.Internal;

public class PlayerClimbing : MonoBehaviour
{   
    public enum PlayerState
    {
        WALKING,
        FALLING,
        CLIMBING
    }

    [Header("State Settings")] [SerializeField]
    public PlayerState playerState = PlayerState.WALKING;

    // [SerializeField] private float walkSpeed = 3f;
    [SerializeField] float climbingSpeed = 3f;
    [SerializeField] float snapSpeed = 5f;

    [Header("Player References")] public CharacterController controller;
    public ThirdPersonMovement movement;

    private Vector2 inputDirection;
    private bool jumpTrigger = false;
    //bool jumpDown = false;

    private void Start()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (movement == null) movement = GetComponent<ThirdPersonMovement>();
    }

    
        void Update()
        {
            inputDirection = movement.moveAction.action.ReadValue<Vector2>();
            jumpTrigger = movement.jumpAction.action.triggered;

            Vector2 input = SquareToCircle(inputDirection);

            switch (playerState)
            {
                case PlayerState.WALKING:
                    HandleWalkingState();
                    break;

                case PlayerState.FALLING:
                    HandleFallingState();
                    break;

                case PlayerState.CLIMBING:
                    HandleClimbingState(input);
                    break;
            }

            if (playerState != PlayerState.CLIMBING)
            {
                playerState = controller.isGrounded
                    ? PlayerState.WALKING
                    : PlayerState.FALLING;
            }

            jumpTrigger = false;
        }
        /*
        //    h = Input.GetAxis("Horizontal");
        //  v = Input.GetAxis("Vertical");
        if (movement != null && movement.moveAction != null && movement.moveAction.action != null)
        {
            inputDirection = movement.moveAction.action.ReadValue<Vector2>();
        }

        if (!jumpTrigger && movement != null && movement.jumpAction != null && movement.jumpAction.action != null)
        {
            jumpTrigger = movement.jumpAction.action.triggered;
        }*/
    

   /* void FixedUpdate()
    {

        Vector2 input = SquareToCircle(inputDirection);
        //Transform cam = Camera.main.transform;
      //  Vector3 moveDirection = Quaternion.FromToRotation(cam.up, Vector3.up)
                         //       * cam.TransformDirection(new Vector3(input.x, 0, input.y));

        switch (playerState)
        {
            case PlayerState.WALKING:
                HandleWalkingState();
                break;
            case PlayerState.FALLING:
                HandleFallingState();
                break;
            case PlayerState.CLIMBING:
                HandleClimbingState(input);
                break;
        }

        // private RaycastHit hit;

        // if (Physics.Raycast(transform.position, Vector3.down. out hit, 1.02f))
        //  state = PlayerState.WALKING;
        //  else IFormattable(state == PlayerState.WALKING)
        //  state = PlayerState.FALLING;
        
        // Ground/fall check if not climbing
        if (playerState != PlayerState.CLIMBING)
        {
            //  if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f))
            // {
            if (controller.isGrounded)
            {
                playerState = PlayerState.WALKING;
            }
            else
            {
                playerState = PlayerState.FALLING;
            }
        }

        // clear jump
        jumpTrigger = false;
    }
*/

    


void HandleWalkingState()
{
   // Debug.Log("WALKING");
    // if walking, re-enable
    if(!movement.enabled) movement.enabled = true;
    
    // transition to climb
    if (inputDirection.y > 0.1f && Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 0.7f))
    {
        // check if object climbable
        // ADD TAG Climbable
        if (hit.transform.CompareTag("Climbable"))
        {
            Debug.Log($"Walking Input: {inputDirection}");
            playerState = PlayerState.CLIMBING;
        }
    }
}

/*void HandleWalking(Vector3 moveDirection)
{
    Vector3 oldVelocity = ThirdPersonMovement.velocity;
    Vector3 newVelocity = moveDirection * walkSpeed;
    newVelocity.y = oldVelocity.y;
    if (jumpDown)
    {
        newVelocity.y = 5f;
        state = PlayerClimbing.PlayerState.FALLING
    }

    ThirdPersonMovement.velocity = newVelocity;
    if (moveDirection.sqrMagnitude > 0.01f)
    {
        Transform.forward = moveDirection;
    }

}
*/
void HandleFallingState()
{
   // Debug.Log("FALLING");
    if(!movement.enabled) movement.enabled = true;
    
    // Air grab
    if (inputDirection.y > 0.1f && Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 0.6f))
    {
        Debug.Log($"Walking Input: {inputDirection}");
        playerState = PlayerState.CLIMBING;
    }
    
   // if (jumpDown && Physics.Raycast(transform.position, transform.forward * 0.4f))
      //  state = PlayerClimbing.PlayerState.CLIMBING;
}

void HandleClimbingState(Vector2 input)
{
    //if(movement.enabled) movement.canMove = false;
    
    Vector3 origin = transform.position + transform.up * 0.5f;
    
    // Find the wall directly in front of the player
    if (!Physics.Raycast(origin, transform.forward, out RaycastHit hit, 1.2f))
    {
        playerState = PlayerState.FALLING;
        return;
    }

    Vector3 wallNormal = hit.normal;

    // Face the wall
    Quaternion targetRotation = Quaternion.LookRotation(-wallNormal, Vector3.up);
    transform.rotation = Quaternion.Slerp(
        transform.rotation,
        targetRotation,
        15f * Time.deltaTime);

    //--------------------------------------------------
    // Keep the player a fixed distance from the wall
    //--------------------------------------------------

    float desiredDistance = controller.radius * 0.9f;

    float error = hit.distance - desiredDistance;

    Vector3 snapMovement = -wallNormal * error * snapSpeed;

    //--------------------------------------------------
    // Calculate climbing movement
    //--------------------------------------------------

    // Up direction
    Vector3 wallUp = Vector3.up;

    // Sideways direction along the wall
    Vector3 wallRight = Vector3.Cross(wallUp, wallNormal).normalized;

    Vector3 climbMovement =
        wallRight * input.x +
        wallUp * input.y;

    climbMovement *= climbingSpeed;

    //--------------------------------------------------
    // Combine movement
    //--------------------------------------------------

    Vector3 finalMovement = climbMovement + snapMovement;

    controller.Move(finalMovement * Time.deltaTime);

    //--------------------------------------------------
    // Jump away
    //--------------------------------------------------

    if (jumpTrigger)
    {
       // movement.canMove = true;

        //movement.VerticalVelocity =
           // Mathf.Sqrt(movement.jumpHeight * 2f * movement.gravity);

        playerState = PlayerState.FALLING;
    }
    
    /*
    Debug.Log($"Climbing Input: {input}");
   // Debug.Log("CLIMBING");
    if(movement.enabled) movement.enabled = false;
    
    Vector3 offset = transform.TransformDirection(new Vector3(0.5f, 0.5f, 0f));
    Vector3 checkDirection = Vector3.zero;
    int hitCount = 0;

    for (int i = 0; i < 4; i++)
    {
        if (Physics.Raycast(transform.position + offset, transform.forward, out RaycastHit checkHit, 1f))
        {
            checkDirection += checkHit.normal;
            hitCount++;
        }
        // Rotate offset
        offset = Quaternion.AngleAxis(90f, transform.forward) * offset;
    }
    // average wall vector or fallback to player reverse movement
    Vector3 wallNormal = (hitCount > 0) ? (checkDirection / hitCount).normalized : -transform.forward;
    
    // check player facing straight ahead to snap distant to wall surface
    if (Physics.Raycast(transform.position, -wallNormal, out RaycastHit mainHit, 1.5f))
    {
        Quaternion targetRotation = Quaternion.LookRotation(-wallNormal, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 15f * Time.deltaTime);
        //Vector3 targetPosition = mainHit.point + wallNormal * 0.45f; // match player radius
        //transform.position = Vector3.Lerp(transform.position, targetPosition, 10f * Time.fixedDeltaTime);
        
        // lock alignment to wall
        
        // Vector3 targetPosition = mainHit.point + wallNormal * 0.45f;
        // Vector3 snapVelocity = (targetPosition - transform.position) * snapSpeed;
        //Quaternion targetRotation = Quaternion.LookRotation(-wallNormal, Vector3.up);
       // transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        
        // mapping new coordinates to up/down/sideways plane coordination
        Debug.Log($"transform.up = {transform.up}");
        Debug.Log($"transform.right = {transform.right}");
        Vector3 climbVelocity = Vector3.up;
      // Vector3 climbVelocity = (transform.up * input.y) + (transform.right * input.x);
       // climbVelocity *= climbingSpeed;
        Debug.Log(transform.position);
        //Vector3 finalMovement = climbVelocity + snapVelocity;
        controller.Move(climbVelocity * Time.deltaTime);
        Debug.Log(transform.position);
        Debug.Log(climbVelocity);
       // controller.Move(climbVelocity *(climbingSpeed * Time.fixedDeltaTime));
        
        // Jump off wall check
        if (jumpTrigger)
        {
            movement.enabled = true;
            movement.VerticalVelocity = Mathf.Sqrt(movement.jumpHeight * 1.5f * movement.gravity);

            Vector3 leapDirection = (wallNormal * 3f) + (transform.up * 2f);
            controller.Move(leapDirection * Time.fixedDeltaTime);
            
            playerState = PlayerState.FALLING;
        }
    }
    else
    {
        playerState = PlayerState.FALLING;
    }
    /*
    // Check walls
    Vector3 offset = transform.TransformDirection(Vector2.one * 0.5f);
    Vector3 checkDirection = Vector3.zero;
    int k = 0;
    for (int i = 0; i < 4; i++)
    {
        RaycastHit checkHit;
        if (Physics.Raycast(transform.position + offset, transform.forward, out checkHit))
        {
            checkDirection += checkHit.normal;
            k++;
        }
        // Rotate offset by 90 degrees
        offset = Quaternion.AngleAxis(90f, transform.forward) * offset;
    }

    checkDirection /= k;

    // Check wall directly in front
    RaycastHit hit;
    if (Physics.Raycast(transform.position, -checkDirection, out hit))
    {
        float dot = Vector3.Dot(transform.forward, -hit.normal);
        ThirdPersonMovement.position = Vector3.Lerp(ThirdPersonMovement.position, hit.point + hit.normal * 0.55f, 5f * Time.fixedDeltaTime);
        transform.forward = Vector3.Lerp(transform/ForwardLights. -hit.normal, 10f * Time.fixedDeltaTime);

        ThirdPersonMovement.useGravity = false;
        ThirdPersonMovement.velocity = transform.TransformDirection(input) * climbSpeed;
        if (jumpTrigger)
        {
            ThirdPersonMovement.velocity = Vector3.up * 5f + hit.normal * 2f;
            playerState = PlayerState.FALLING;
        }
    }
    else
    {
        playerState = PlayerState.FALLING;
    }
    */
}



Vector2 SquareToCircle(Vector2 input)
{
    return(input.sqrMagnitude >= 1f ? input.normalized : input);
}

}

