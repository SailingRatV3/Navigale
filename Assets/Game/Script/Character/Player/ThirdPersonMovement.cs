using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ThirdPersonMovement : MonoBehaviour
{

    public enum MovementState
    {
        NORMAL,
        CLIMBING,
        MANTLING,
        FALLING,
        CROUCHING // Add crouching mechanic later
    }
    
   public MovementState currentState = MovementState.NORMAL;
    
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
   
   private float verticalVelocity;

   public float VerticalVelocity
   {
       get => verticalVelocity;
       set => verticalVelocity = value;
   }
   
   bool isGrounded;
   public bool canMove = true;
   
   [Header("Climbing Settings")]
   public LayerMask climbingLayer;
   public float climbSpeed = 2f;
   public float wallStickForce = 0.05f;
   public float wallJumpForce = 1f;
   public float wallCheckDistance = 1f;
   public float wallNormalSmooth = 10f;
   
   private RaycastHit climbHit;
   private bool isClimbing;
   private Vector3 currentWallNormal;

   [Header("Ledge Settings")] 
   public float ledgeCheckHeight = 1.8f;
   public float ledgeForwardDistance = 0.5f;
   public float ledgeClimbDuration = 0.35f;

   private bool isMantling;
   private Vector3 mantleStart;
   private Vector3 mantleEnd;
   private float mantleTimer;

   bool CheckForClimbableWall()
   {
       Vector3 origin = transform.position + transform.up;
       // This allows to climb on single surfaces
       /*
       return Physics.Raycast(
           origin,
           transform.forward,
           out climbHit,
           wallCheckDistance,
           climbingLayer
       );*/
       // This allows to climb around corners / curved surface
       // Checking forward
       if (Physics.SphereCast(
               origin,
               0.4f,
               transform.forward,
               out climbHit,
               wallCheckDistance,
               climbingLayer
           ))
       {
           return true;
       }
       
       // Checking Left
       if (Physics.SphereCast(
               origin,
               0.4f,
               -transform.right,
               out climbHit,
               wallCheckDistance,
               climbingLayer
           ))
       {
           return true;
       }
       //Checking Right
       if (Physics.SphereCast(
               origin,
               0.4f,
               transform.right,
               out climbHit,
               wallCheckDistance,
               climbingLayer
           ))
       {
           return true;
       }
       
       return false;
   }
   
   bool CheckForLedge(out Vector3 targetPos)
   {
       bool blocked = Physics.CheckCapsule(
           mantleEnd + Vector3.up * 0.5f, mantleEnd + Vector3.up * controller.height, controller.radius);
       
       
       targetPos = Vector3.zero;
       // From player chest to ledge
       Vector3 chest = transform.position + Vector3.up * ledgeCheckHeight;
       // Check if there is nothing above player chest and up (ex: wall edge)
       if (Physics.Raycast(
               chest,
               Vector3.forward,
               wallCheckDistance,
               climbingLayer))
       {
           return false;
       }
       
       // Check above ground
       Vector3 above = chest + transform.forward * ledgeForwardDistance + Vector3.up;

       if (Physics.Raycast(
               above,
               Vector3.down,
               out RaycastHit hit,
               2f))
       {
           targetPos = hit.point + Vector3.up * (controller.height * 0.5f);

           return true;
       }

       if (Vector3.Angle(hit.normal, Vector3.up) > 45f) return false;

       if (blocked) return false; 
       
       return false;
   }
   
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
       
       // States
       switch (currentState)
       {
           case MovementState.NORMAL:
               NormalMovement();
               break;
           case MovementState.CLIMBING:
               ClimbMovement();
               break;
           case MovementState.MANTLING:
               MantleMovement();
               break;
           case MovementState.FALLING:
               break;
           case MovementState.CROUCHING:
               CrouchMovement();
               break;
       }
    
   }

   void NormalMovement()
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

       if (canMove && direction.magnitude >= 0.1f)
       {
           float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
       
           float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        
           transform.rotation = Quaternion.Euler(0f, angle, 0f);
        
           Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
           controller.Move(moveDir.normalized * (currentSpeed * Time.deltaTime));
       }

       if (canMove && jumpAction.action.triggered && isGrounded)
       {
           verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
       }
    
       verticalVelocity -= gravity * Time.deltaTime;
    
       controller.Move(Vector3.up * (verticalVelocity * Time.deltaTime));
       
       // Climbing State
       if (currentState == MovementState.NORMAL)
       {
           if (CheckForClimbableWall() && !isGrounded)
           {
               currentState = MovementState.CLIMBING;
               verticalVelocity = 0;
           }

           if (CheckForClimbableWall() && jumpAction.action.IsPressed())
           {
               currentState = MovementState.CLIMBING;
           }
       }
   }

   void ClimbMovement()
   {
       if (!CheckForClimbableWall())
       {
           currentState = MovementState.NORMAL;
           return;
       }

       // if ledge is located
       if (CheckForLedge(out Vector3 ledgePos))
       {
           currentState = MovementState.MANTLING;

           mantleStart = transform.position;
           mantleEnd = ledgePos;

           mantleTimer = 0f;

           return;
       }
       
       // Look at wall
       
       // Snaps to walls
       // Quaternion targetRotation = Quaternion.LookRotation(-climbHit.normal);
       // Stores wall normals
       currentWallNormal = Vector3.Slerp(currentWallNormal, climbHit.normal, wallNormalSmooth * Time.deltaTime);
       Quaternion targetRotation = Quaternion.LookRotation(-currentWallNormal);
       transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 12f * Time.deltaTime);
     
       // Inputs
       Vector2 input = moveAction.action.ReadValue<Vector2>();
       
       // Cam Movement -- Maybe replace player position to camera???
       Vector3 cameraForward = Vector3.ProjectOnPlane(cam.forward, currentWallNormal).normalized;
       Vector3 cameraRight = Vector3.ProjectOnPlane(cam.right, currentWallNormal).normalized;
       
       // Movement
       Vector3 up = Vector3.ProjectOnPlane(Vector3.up, currentWallNormal).normalized;
       Vector3 right = Vector3.Cross(currentWallNormal, up).normalized;
       Vector3 move = (up * input.y) + (right * input.x);
       controller.Move(move * climbSpeed * Time.deltaTime);
       
       // Cling to the wall
       controller.Move(-currentWallNormal * wallStickForce);

       // Wall jump
       if (jumpAction.action.triggered)
       {
           currentState = MovementState.NORMAL;
           
           Vector3 jumpDirection = climbHit.normal + Vector3.up;
           
           controller.Move(jumpDirection.normalized * wallJumpForce);
           
           verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
       }
   }

  

   void MantleMovement()
   {
      mantleTimer += Time.deltaTime;
      float t = mantleTimer / ledgeClimbDuration;
      // disable
      controller.enabled = false;
      
      Vector3 pos = Vector3.Lerp(mantleStart, mantleEnd, t);
      pos.y += Mathf.Sin(t * Mathf.PI) * 0.3f;
      // move player over ledge
      // This moves a curve (slightly)
      transform.position = pos;
      //This moves a straight line
      // transform.position = Vector3.Lerp(mantleStart, mantleEnd, Mathf.SmoothStep(0f,1f,t));
      // enable
      controller.enabled = true;

      if (t >= 1f)
      {
          currentState = MovementState.NORMAL;
      }
   }
   

   void CrouchMovement()
   {
       
   }
   
}
/*
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : MonoBehaviour
{
    public enum PlayerState
    {
        WALKING,
        FALLING,
        CLIMBING
    }

    [Header("References")]
    public CharacterController controller;
    public Transform cam;

    [Header("Input")]
    public InputActionReference moveAction;
    public InputActionReference runAction;
    public InputActionReference jumpAction;

    [Header("Walking")]
    public float speed = 6f;
    public float runMultiplier = 1.8f;
    public float turnSmoothTime = 0.1f;

    [Header("Jump")]
    public float gravity = 25f;
    public float jumpHeight = 3f;

    [Header("Climbing")]
    public float climbingSpeed = 3f;
    public float climbDetectDistance = 1f;
    public float wallSnapSpeed = 5f;
    public float wallDistance = 0.45f;
    public float wallJumpForce = 5f;

    private float turnSmoothVelocity;
    private float verticalVelocity;

    private bool isGrounded;
    private bool isRunning;

    private Vector2 input;
    private PlayerState state = PlayerState.WALKING;

    private RaycastHit currentWall;
    
    private Vector3 climbWallNormal;
    private Vector3 targetWallNormal;
    public float wallNormalSmoothSpeed = 10f;


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
        input = moveAction.action.ReadValue<Vector2>();

        isGrounded = controller.isGrounded;

        Vector3 movement = Vector3.zero;


        switch (state)
        {
            case PlayerState.WALKING:
                movement = Walking();
                break;

            case PlayerState.FALLING:
                movement = Falling();
                break;

            case PlayerState.CLIMBING:
                movement = Climbing();
                break;
        }


        controller.Move(movement * Time.deltaTime);


        // State checks
        if (state != PlayerState.CLIMBING)
        {
            if (isGrounded)
                state = PlayerState.WALKING;
            else
                state = PlayerState.FALLING;
        }


        // Try grabbing wall
        if (state != PlayerState.CLIMBING)
        {
            TryClimb();
        }
    }


    Vector3 Walking()
    {
        Vector3 movement = Vector3.zero;


        Vector3 direction = new Vector3(input.x, 0, input.y);


        isRunning = runAction.action.IsPressed();

        float currentSpeed =
            isRunning ? speed * runMultiplier : speed;


        if (direction.magnitude > 0.1f)
        {
            float targetAngle =
                Mathf.Atan2(direction.x, direction.z)
                * Mathf.Rad2Deg
                + cam.eulerAngles.y;


            float angle =
                Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    targetAngle,
                    ref turnSmoothVelocity,
                    turnSmoothTime);


            transform.rotation =
                Quaternion.Euler(0, angle, 0);


            Vector3 moveDirection =
                Quaternion.Euler(0, targetAngle, 0)
                * Vector3.forward;


            movement += moveDirection.normalized * currentSpeed;
        }


        if (jumpAction.action.triggered && isGrounded)
        {
            verticalVelocity =
                Mathf.Sqrt(jumpHeight * 2f * gravity);
        }


        verticalVelocity -= gravity * Time.deltaTime;

        movement.y = verticalVelocity;


        return movement;
    }



    Vector3 Falling()
    {
        Vector3 movement = Vector3.zero;


        verticalVelocity -= gravity * Time.deltaTime;

        movement.y = verticalVelocity;


        return movement;
    }



    void TryClimb()
    {
        if (input.y <= 0.1f)
            return;


        if (Physics.Raycast(
            transform.position,
            transform.forward,
            out RaycastHit hit,
            climbDetectDistance))
        {
            if (hit.transform.CompareTag("Climbable"))
            {
                currentWall = hit;
                state = PlayerState.CLIMBING;
                verticalVelocity = 0;
            }
        }
    }



    Vector3 Climbing()
    {
        Vector3 movement = Vector3.zero;


        if (!Physics.Raycast(
            transform.position,
            transform.forward,
            out RaycastHit hit,
            climbDetectDistance))
        {
            state = PlayerState.FALLING;
            return movement;
        }


        currentWall = hit;


        Vector3 wallNormal = currentWall.normal;


        // Face wall
        Quaternion targetRotation =
            Quaternion.LookRotation(
                -wallNormal,
                Vector3.up);


        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                15f * Time.deltaTime);



        // Keep distance from wall
        float distanceError =
            currentWall.distance - wallDistance;


        movement +=
            -wallNormal *
            distanceError *
            wallSnapSpeed;



        // Movement directions on wall
        Vector3 wallRight =
            Vector3.Cross(
                Vector3.up,
                wallNormal)
            .normalized;


        Vector3 climbDirection =
            wallRight * input.x +
            Vector3.up * input.y;


        movement +=
            climbDirection * climbingSpeed;



        // Jump off wall
        if (jumpAction.action.triggered)
        {
            state = PlayerState.FALLING;

            movement +=
                wallNormal * wallJumpForce
                + Vector3.up * wallJumpForce;
        }


        return movement;
    }
}
 */
