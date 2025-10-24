using System;
using Unity.VisualScripting;
using UnityEngine;
using Object = System.Object;

public class Character : MonoBehaviour
{
    #region Properties & References
    
    [Header("References")]
    public CharacterMovementData MovementData;
    public Rigidbody2D rb;
    public Collider2D col;
    public Collider2D parryCollider2D;
    public Collider2D attackCollider2D;
    
    [Header("State Machine")]
    public CharacterStateMachine StateMachine;
    public CharacterIdleState IdleState;
    public CharacterWalkState WalkState;
    public CharacterAirState AirState;
    public CharacterJumpState JumpState;
    public CharacterDashState DashState;
    public CharacterSwitchDashState SwitchDashState;
    public CharacterJumpParryState JumpParryState;
    public CharacterSlashState SlashState;
    public CharacterEyeState EyeState;

    
    //[Header("Events")] 
    public event Action<string> CharacterStateChange;
    public void AddCharacterStateObserver(Action<string> observer) { CharacterStateChange += observer; }
    public void RemoveCharacterStateObserver(Action<string> observer) { CharacterStateChange -= observer; }

    
    [Header("Debug")]
    public bool ShowEnteredStateDebugLog;
    public bool ShowGroundedCollisionBox;
    public bool ShowCeilingCollisionBox;

    #endregion
    
    #region Events
    public static event Action<string> StateChanged;

    public static void AddControlsObserver(Action<string> observer) { StateChanged += observer; }
    public static void RemoveControlsObserver(Action<string> observer) { StateChanged -= observer; }

    public void StateChangeEvent(Character obj, string state)
    {
        obj.CharacterStateChange.Invoke(state);
    }

    #endregion
    private void Awake()
    {
        StateMachine = new CharacterStateMachine();
        IdleState = new CharacterIdleState(this);
        WalkState = new CharacterWalkState(this);
        AirState = new CharacterAirState(this);
        JumpState = new CharacterJumpState(this);
        DashState = new CharacterDashState(this);
        SwitchDashState = new CharacterSwitchDashState(this);
        SlashState = new CharacterSlashState(this);
        JumpParryState = new CharacterJumpParryState(this);
        EyeState = new CharacterEyeState(this);

    }
    
    private void Start()
    {
        StateMachine.InitializeDefaultState(IdleState);
    }
    
    private void Update()
    {
        StateMachine.CurrentState.StateUpdate();
    }
    
    private void FixedUpdate()
    {
        StateMachine.CurrentState.StateFixedUpdate();
    }
    
    #region Physics
    
    public float HorizontalVelocity { get; private set; }
    public float VerticalVelocity { get; private set; }
    
    public void ApplyVelocity()
    {
        rb.linearVelocity = new Vector2(HorizontalVelocity, VerticalVelocity);
    }

    public void SetVerticalVelocity(float velocity) => VerticalVelocity = velocity;
    public void IncrementVerticalVelocity(float increment) => VerticalVelocity += increment;

    #endregion
    
    #region Timers and Cooldowns

    public void TickTimers()
    {
        TickJumpBufferTimer();
        TickCoyoteTimeTimer();
        TickJumpApexTimer();
        TickDashTimers();
        TickSwitchDashTimer();
    }
    
    #endregion
    
    #region Collision Detection
    
    private RaycastHit2D _groundHit;
    public bool IsGrounded { get; private set; }
    private RaycastHit2D _ceilingHit;
    public bool IsTouchingCeiling { get; private set; }
    public bool InParryZone { get; private set; }
    
    public bool AttackParried { get; private set; }


    public void CollisionChecks()
    {
        CheckForGrounded();
        CheckForCeiling();
        CheckForParryableObject();
    }

    public void CheckForGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(col.bounds.center.x, col.bounds.min.y);
        Vector2 boxCastSize = new Vector2(col.bounds.size.x * MovementData.groundDetectionRayWidth, MovementData.groundDetectionRayLength);
        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MovementData.groundDetectionRayLength, MovementData.groundLayer);
        IsGrounded = _groundHit.collider;
        
        #region Debug Visualization
        if (ShowGroundedCollisionBox)
        {
            Color rayColor = IsGrounded ? Color.green : !IsJumping && !IsFastFalling && CoyoteTimer > 0 ? Color.yellow : Color.red;
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MovementData.groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MovementData.groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - MovementData.groundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        }
        #endregion
    }
    
    public void CheckForParryableObject()
    {
        float radius = .5f;
        Vector2 parryColOrigin = parryCollider2D.bounds.center;
        Collider2D[] otherCol = Physics2D.OverlapCircleAll(parryColOrigin, radius, ~0);
        InParryZone = false;
        for (int i = 0; i < otherCol.Length; i++)
        {
            if (otherCol[i].gameObject.TryGetComponent(out IParryable parryable))
            {
                // if (parryable.ParryableNow)
                // {
                    Debug.Log("Parryable Object Collided!");
                    InParryZone = true;
                // }
            }
        }
    }

    public void CheckForCeiling()
    {
        Vector2 boxCastOrigin = new Vector2(col.bounds.center.x, col.bounds.max.y);
        Vector2 boxCastSize = new Vector2(col.bounds.size.x * MovementData.ceilingDetectionRayWidth, MovementData.ceilingDetectionRayLength);
        _ceilingHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, MovementData.ceilingDetectionRayLength, MovementData.groundLayer);
        IsTouchingCeiling = _ceilingHit.collider;
        
        #region Debug Visualization
        if (ShowCeilingCollisionBox)
        {
            Color rayColor = IsTouchingCeiling ? Color.red : Color.green;
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.up * MovementData.ceilingDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.up * MovementData.ceilingDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + MovementData.ceilingDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        }
        #endregion
    }
    
    #endregion
    
    #region Walking and Dashing

    public bool IsFacingRight { get; private set; } = true;
    
    #region Timers

    public float DashTimer { get; private set; }
    public float DashCooldown { get; private set; }

    //amount of time that can pass after Dash is activated before Switchdash can no longer be activated
    public float SwitchDashCooldown { get; private set; }

    //amount of time Switchdash lasts (after regular dash)
    public float SwitchDashTimer { get; private set; }
    
    public void InitializeDashTimers()
    {
        DashTimer = MovementData.dashDuration;
        DashCooldown = MovementData.dashCooldown;
        SwitchDashCooldown = MovementData.switchdashCooldown;
    }
    
    public void InitializeSwitchDashTimer()
    {
        SwitchDashTimer = MovementData.switchdashDuration;
    }

    private void TickDashTimers()
    {
        if (StateMachine.CurrentState == DashState)
        {
            DashTimer -= Time.deltaTime;
            SwitchDashCooldown -= Time.deltaTime;
        }
        else
            DashCooldown -= Time.deltaTime;

        DashTimer = Mathf.Clamp(DashTimer, 0, MovementData.dashDuration);
        DashCooldown = Mathf.Clamp(DashCooldown, 0, MovementData.dashCooldown);
        SwitchDashCooldown = Mathf.Clamp(SwitchDashTimer, 0, MovementData.dashCooldown);
    }
    
    private void TickSwitchDashTimer()
    {
        if (StateMachine.CurrentState == SwitchDashState)
            SwitchDashTimer -= Time.deltaTime;
        
        SwitchDashTimer = Mathf.Clamp(SwitchDashTimer, 0, MovementData.switchdashDuration);
    }

    #endregion
    
    #region Functions
    
    public void Move(Vector2 direction, float acceleration, float deceleration)
    {
        bool isMoving = Mathf.Abs(direction.x) > MovementData.moveThreshold;
        if (isMoving)
        {
            CharacterStateChange.Invoke("Move");
            CheckForFlip(direction);
            float targetVelocity = direction.x * MovementData.maxWalkSpeed;
            HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            CharacterStateChange.Invoke("Idle");
            HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, 0f, deceleration * Time.deltaTime);
        }
    }
    
    public void Dash(Vector2 direction, float velocity)
    {
        CharacterStateChange.Invoke("Dash");
        CheckForFlip(direction);
        HorizontalVelocity = direction.x * velocity;
        SetVerticalVelocity(0);
    }
    
    private void CheckForFlip(Vector2 moveInput)
    {
        bool movingRight = moveInput.x > 0;
        if (IsFacingRight != movingRight)
        {
            IsFacingRight = movingRight;
            transform.Rotate(0f, movingRight ? 180f : -180f, 0f);
        }
    }
    
    #endregion
    
    #endregion
    
    #region Jumping and Falling

    public bool IsJumping { get; private set; }
    // public bool IsJumpParrying { get; private set; }
    public bool IsFastFalling { get; private set; }
    public float JumpBufferTimer { get; set; }
    public bool JumpReleasedDuringBuffer { get; set; }
    public float CoyoteTimer { get; private set; }
    public float JumpApexPoint { get; private set; }
    public float JumpApexTimer { get; private set; }
    public bool JumpApexTimerStarted { get; set; }
    public float FallTimer { get; private set; }
    
    #region Checks
    
    public bool CanJump() => !IsJumping && !IsFastFalling && (IsGrounded || CoyoteTimer > 0f);
    
    //why doesn't this check if they pressed it this frame??
    // public bool CanJumpParry() => !IsJumpParrying && InParryZone;
    public bool CanJumpParry() => InParryZone;

    #endregion

    #region Timers

    private void TickJumpBufferTimer()
    {
        if (InputManager.GetJumpWasPressedThisFrame())
        {
            JumpBufferTimer = MovementData.jumpBufferTime;
            JumpReleasedDuringBuffer = false;
        }
        else
        {
            JumpBufferTimer -= Time.deltaTime;
        }

        if (InputManager.GetJumpWasReleasedThisFrame() && JumpBufferTimer > 0)
            JumpReleasedDuringBuffer = true;
        
        JumpBufferTimer = Mathf.Clamp(JumpBufferTimer, 0, MovementData.jumpBufferTime);
    }

    private void TickCoyoteTimeTimer()
    {
        if (IsGrounded)
            CoyoteTimer = MovementData.jumpCoyoteTime;
        else
            CoyoteTimer -= Time.deltaTime;
        
        CoyoteTimer = Mathf.Clamp(CoyoteTimer, 0, MovementData.jumpCoyoteTime);
    }

    private void TickJumpApexTimer()
    {
        if (!IsJumping)
        {
            JumpApexTimer = 0;
            JumpApexTimerStarted = false;
        }
        else
        {
            JumpApexPoint = Mathf.InverseLerp(MovementData.InitialJumpVelocity, 0f, Mathf.Abs(VerticalVelocity));
            if (!JumpApexTimerStarted && JumpApexPoint > MovementData.apexThreshold)
            {
                JumpApexTimer = MovementData.apexHangTime;
                JumpApexTimerStarted = true;
            }
            else
                JumpApexTimer -= Time.deltaTime;
        }
        
        JumpApexTimer = Mathf.Clamp(JumpApexTimer, 0, MovementData.apexHangTime);
    }
    
    #endregion
    
    #region Functions

    public void Jump()
    {
        CharacterStateChange.Invoke("Jump");
        SetVerticalVelocity(MovementData.InitialJumpVelocity);
        IsJumping = true;
        IsFastFalling = false;
        JumpBufferTimer = 0;
    }

    public void CancelJumpEarly()
    {
        IsJumping = false;
        IsFastFalling = true;
        JumpReleasedDuringBuffer = false;
    }
    
    public void Land()
    {
        //may make a land animation
        CharacterStateChange.Invoke("Idle");
        SetVerticalVelocity(0);
        IsJumping = false;
        IsFastFalling = false;
    }
    
    public void JumpParry()
    {
        CharacterStateChange.Invoke("JumpParry");
        SetVerticalVelocity(MovementData.InitialJumpVelocity);
        // IsJumpParrying = true;
        IsFastFalling = false;
        //JumpParryBufferTimer = 0;
    }

    
    public void AirPhysics()
    {
        if (IsJumping)
        {
            if (IsTouchingCeiling)
            {
                SetVerticalVelocity(0);
                IsJumping = false;
                IsFastFalling = true;
            }
            else if (JumpApexTimerStarted)
            {
                if (JumpApexTimer > 0)
                {
                    SetVerticalVelocity(0f);
                }
                else
                {
                    IsJumping = false;
                    IsFastFalling = true;
                }
            }
            else
            {
                IncrementVerticalVelocity(MovementData.Gravity * Time.fixedDeltaTime);
            }
        }

        if (IsFastFalling)
        {
            IncrementVerticalVelocity(MovementData.Gravity * MovementData.gravityOnReleaseMultiplier * Time.fixedDeltaTime);
        }
        else
            IncrementVerticalVelocity(MovementData.Gravity * Time.fixedDeltaTime);
    }
    
    #endregion
    
    #endregion
    
    #region Combat
    
    public void Slash()
    {
        CharacterStateChange.Invoke("Slash");
        Debug.Log("Pressed slash");

        float radius = .5f;
        
        //need to find if the player has a weapon in hand
        //if no weapon, the arm of the character is the weapon
        Vector2 weaponColOrigin = attackCollider2D.bounds.center;
        Collider2D[] otherCol = Physics2D.OverlapCircleAll(weaponColOrigin, radius, ~0);
        for (int i = 0; i < otherCol.Length; i++)
        {
            Debug.Log("****Some Object Collided..");

            if (otherCol[i].gameObject.TryGetComponent(out IDamagable damagable))
            {
                Debug.Log("****Damagable Object Collided!");
                damagable.TakeDamage(1);
            }
        }
    }
    
    #endregion
}
