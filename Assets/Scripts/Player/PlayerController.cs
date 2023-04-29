using Rayqdr.Inputs;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

/*
 *              With the help of @DawnosaurDev at youtube.com/c/DawnosaurStudios
 */

public class PlayerController : MonoBehaviour
{
    public event Action OnDropDown;
    private enum State
    {
        Normal,
        Dash,
        Roll
    }
    private State state;  

    #region Variables
    [Tooltip("Put here the desired Movement Data")]
    [SerializeField] private PlayerData Data;

    [SerializeField] private float dashCooldown = 1.0f;
    private float nextDashTime = 0.0f;
    private Vector2 dashDir;
    private float dashDuration;
    private float dashBufferCount;
    private bool hasDashed = false;

    [SerializeField] private float rollCooldown = 0.3f;
    private float nextRollTime = 0.0f;

    private PlayerScript player;
    private Animator animator;


    private PlayerInput inputAction;

    private Rigidbody2D rb;

    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    private Collider2D colliderRef;
    private bool canDropDown;
    //Timers (also all fields, could be private and a method returning a bool could be used)
    public float LastOnGroundTime { get; private set; }
    private float baseJumpBufferTime;
    private const float FIRST_JUMP_MULTIPLIER = 1;
    private const float SECOND_JUMP_MULTIPLIER = 0.5f;

    private Vector2 _moveInput;

    //Jump
    private bool _isJumpCut;
    private bool _isJumpFalling;

    public float LastPressedJumpTime { get; private set; }

    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;

    //Set all of these up in the inspector
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize;
    private Vector2 storedVelocity;

    private InputAction jumpAction;
    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction dashAction;
    private InputAction rollAction;

    private GameObject graphObject;
    private Collider2D collisionBox;

    private Coroutine dashCoroutine;
    #endregion
    [SerializeField] private ParticleHandle particles;
    private bool playRunParticle = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collisionBox = this.GetComponent<CapsuleCollider2D>();

        player = GetComponent<PlayerScript>();
        graphObject = this.transform.GetChild(0).GetChild(0).gameObject;
        #region Input Related Stuff
        inputAction = GetComponent<PlayerInput>();

        jumpAction = inputAction.actions["Jump"];
        moveAction = inputAction.actions["Move"];
        interactAction = inputAction.actions["Interact"];
        dashAction = inputAction.actions["Dash"];
        rollAction = inputAction.actions["Roll"];
        jumpAction.started += Jump_started;
        jumpAction.canceled += Jump_canceled;
        dashAction.performed += DashAction_performed;
        rollAction.performed += RollAction_performed;
        #endregion
    }

    #region InputActionEvents
    private void Jump_started(InputAction.CallbackContext obj)
    {
        //Check si sur une platforme collider, si oui, descendre de la platforme
        if (CanDropDownFromPlatform() && OnDropDown != null)
        {
            OnDropDown();
            return;
        }
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }

    private void Jump_canceled(InputAction.CallbackContext obj)
    {
        if (CanJumpCut())
            _isJumpCut = true;
    }

    private void DashAction_performed(InputAction.CallbackContext obj)
    {
        dashBufferCount = Data.dashBuffer;
    }

    private void RollAction_performed(InputAction.CallbackContext obj)
    {
        //Perform Roll
        if (CanRoll())
            Roll();
    }
    #endregion

    void Start()
    {
        SetGravityScale(Data.gravityScale);
        IsFacingRight = true;
        animator = player.Animator;
        baseJumpBufferTime = Data.jumpInputBufferTime;
    }

    private void Update()
    {
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;

        dashDuration -= Time.deltaTime;
        dashBufferCount -= Time.deltaTime;

        #endregion
        _moveInput = moveAction.ReadValue<Vector2>();
        switch (state)
        {
            case State.Normal:
                if (Mathf.Abs(_moveInput.x) >= 0.01)
                {
                    CheckDirectionToFace(_moveInput.x > 0);
                    animator.SetBool("IsMoving", true);
                }
                else
                    animator.SetBool("IsMoving", false);
                CheckGrounded();
                CheckJump();
                if (CanDash())
                {
                    dashBufferCount = -1;
                    state = State.Dash;
                    nextDashTime = Time.time + dashCooldown;
                    dashDir = Mathf.Abs(_moveInput.x) >= 0.01 ? _moveInput : new Vector2(this.transform.localScale.x, 0);
                    storedVelocity = this.rb.velocity;
                    dashDuration = Data.dashTime;
                    Dash();
                }
                animator.SetFloat("SpeedY", this.rb.velocity.y);
                break;
            case State.Dash:
                break;
            case State.Roll:
                break;
        }
        SetUpGravity();

        animator.SetBool("IsGrounded", CanJumpFromGround());
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.Dash:
                break;
            case State.Normal:
                Run();
                break;
        }
    }

    private void Run()
    {

        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, 1);

        #region Calculate AccelRate
        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((IsJumping || _isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (Data.doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }
    private void CheckJump()
    {
        if (IsJumping && rb.velocity.y < 0)
        {
            IsJumping = false;
            _isJumpFalling = true;
        }

        if (LastOnGroundTime > 0 && !IsJumping)
        {
            _isJumpCut = false;
            _isJumpFalling = false;
        }

        //Jump
        if (CanJumpFromGround() && LastPressedJumpTime > 0) //can jump if jump has been buffered
        {
            IsJumping = true;
            _isJumpCut = false;
            _isJumpFalling = false;
            Jump(FIRST_JUMP_MULTIPLIER);
            animator.SetTrigger("Jump");
        }
    }
    private void Jump(float multiply)
    {
        //Ensures we can't call Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        //We increase the force applied if we are falling
        //This means we'll always feel like we jump the same amount 
        //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
        float force = Data.jumpForce;
        if (rb.velocity.y < 0)
            force -= rb.velocity.y;

        rb.AddForce(Vector2.up * force * multiply, ForceMode2D.Impulse);
        //Instantier un feedback, particle effect
    }
    private void Dash()
    {
        Data.jumpInputBufferTime = 0.5f;
        if (dashCoroutine != null)
            StopCoroutine(dashCoroutine);
        dashCoroutine = StartCoroutine(DashCoroutine());
    }
    private IEnumerator DashCoroutine()
    {
        float dashStartTime = Time.time;
        IsJumping = false;
        animator.SetTrigger("Dash");

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.drag = 0f;


        while (Time.time <= dashStartTime + Data.dashTime)
        {
            rb.velocity = dashDir.normalized * Data.dashVelocity;
            yield return null;
        }

        state = State.Normal;
        animator.SetTrigger("EndDash");
        rb.velocity /= 2.0f;
        Data.jumpInputBufferTime = baseJumpBufferTime;
        hasDashed = true;
    }
    private void Roll()
    {
        //Passer en state Roll
        state = State.Roll;
        graphObject.GetComponent<SpriteRenderer>().color = Color.green;
        StartCoroutine(RollCoroutine());
    }
    private IEnumerator RollCoroutine()
    {
        animator.SetTrigger("Roll");
        //Enlever les collisions entre les deux joueurs
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, this.gameObject.layer, true);

        float rollStartTime = Time.time;

        rb.velocity = Vector2.zero;
        rb.drag = 0f;

        Vector2 rollDir = new Vector2(this.transform.localScale.x, 0);

        while (Time.time <= rollStartTime + Data.rollTime)
        {
            //perform roll physics
            rb.velocity = rollDir.normalized * Data.rollVelocity;
            yield return null;
        }

        nextRollTime = Time.time + rollCooldown;
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, this.gameObject.layer, false);
        graphObject.GetComponent<SpriteRenderer>().color = Color.white;
        rb.velocity /= 3.0f;
        state = State.Normal;
    }
    private void DropDown()
    {
        if (dropDownCoroutine != null)
            StopCoroutine(dropDownCoroutine);
        dropDownCoroutine = StartCoroutine(HandleCollisions());

        float dropDownPos = 0.1f;
        rb.transform.position = new Vector2(rb.transform.position.x, rb.transform.position.y - dropDownPos);
    }
    private Coroutine dropDownCoroutine;
    IEnumerator HandleCollisions()
    {
        if(colliderRef != null)
        {
            float timeBeforeReEnableCollisions = 0.3f;

            yield return new WaitForSeconds(timeBeforeReEnableCollisions);

        }
    }
    private void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    private void SetUpGravity()
    {
        #region GRAVITY
        switch (state)
        {
            case State.Normal:
                //Higher gravity if we've released the jump input or are falling
                if (rb.velocity.y < 0 && _moveInput.y < 0)
                {
                    //Much higher gravity if holding down
                    SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
                    //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                    rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFastFallSpeed));
                }
                else if (_isJumpCut)
                {
                    //Higher gravity if jump button released
                    SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
                    rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed));
                }
                else if ((IsJumping || _isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold)
                {
                    SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
                }
                else if (rb.velocity.y < 0)
                {
                    //Higher gravity if falling
                    SetGravityScale(Data.gravityScale * Data.fallGravityMult);
                    //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                    rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed));
                }
                else
                {
                    //Default gravity if standing on a platform or moving upwards
                    SetGravityScale(Data.gravityScale);
                }
                break;
            case State.Dash:
                break;
            case State.Roll:
                
                SetGravityScale(Data.gravityScale * Data.rollGravityScale);
                break;
        }
       
        #endregion
    }

    private void CheckGrounded()
    {
        if (!IsJumping)
        {
            var collidersUnderFeet = Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer);
            //Ground Check
            if (collidersUnderFeet) //checks if set box overlaps with ground
            {
                colliderRef = collidersUnderFeet;
                if (!IsJumping)
                {
                    canDropDown = false;
                    if (collidersUnderFeet.TryGetComponent<PlatformEffector2D>(out PlatformEffector2D platform))
                    {
                        canDropDown = true;
                    }
                    LastOnGroundTime = Data.coyoteTime; //if so sets the lastGrounded to coyoteTime
                    Debug.Log(LastOnGroundTime);
                    hasDashed = false;
                }
            }
        }
    }

    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
        {
            Turn();
            particles.PlayEffect("Run",!isMovingRight);
        }
    }
    private void Turn()
    {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }
    private bool CanJumpFromGround() => LastOnGroundTime > 0 && !IsJumping;
    private bool CanJumpCut() => IsJumping && rb.velocity.y > 0;
    private bool HasDashButStillInTheAir() => hasDashed && LastOnGroundTime <= 0.0f; 
    private bool CanDash() => state == State.Normal && Time.time >= nextDashTime && dashBufferCount >= 0.0f && !HasDashButStillInTheAir();
    private bool CanRoll() => state == State.Normal && Time.time >= nextRollTime;
    private bool CanDropDownFromPlatform() => state == State.Normal && canDropDown && _moveInput.y < 0.0f;

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
    #endregion

    private void OnDisable()
    {
        jumpAction.started -= Jump_started;
        jumpAction.canceled -= Jump_canceled;
        dashAction.performed -= DashAction_performed;
    }
}