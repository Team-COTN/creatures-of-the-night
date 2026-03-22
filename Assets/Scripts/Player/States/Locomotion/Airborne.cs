using HSM;
using UnityEngine;

namespace Player.States.Locomotion
{
    public class Airborne : State
    {
        readonly PlayerCharacterController player;
        public readonly Fall Fall;
        public readonly Jump Jump;
        public readonly JumpParry JumpParry;
        public readonly AirDash AirDash;
        public readonly AirSwitchDash AirSwitchDash;
        public bool parried = false;
        // public bool canParry = false;
        public bool CanDash;

        public Airborne(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
            Fall = new Fall(m, this, player);
            Jump = new Jump(m, this, player);
            JumpParry = new JumpParry(m, this, player);
            AirDash = new AirDash(m, this, player);
            AirSwitchDash = new AirSwitchDash(m, this, player);
        }

        protected override State GetDefaultChildState() => Fall;

        protected override (State state, string reason) GetNextState()
        {
            if (InputManager.GetJumpWasPressedThisFrame() && CanParry())
                return (JumpParry, "Player parried object");

            if (Leaf() == AirDash || Leaf() == AirSwitchDash) 
                return (null, null);
            
            if (InputManager.GetDashWasPressedThisFrame() && CanDash)
                return (AirDash, "Player pressed dash");

            return (null, null);
        }

        private bool CanParry()
        {
            var anyMatches = false;
            float radius = 1f;
            Vector2 parryColOrigin = player.parryCollider2D.bounds.center;
            Collider2D[] otherCol = Physics2D.OverlapCircleAll(parryColOrigin, radius, ~0);
            for (int i = 0; i < otherCol.Length; i++)
            {
                if (otherCol[i].gameObject.TryGetComponent(out IParryable parryable) && parryable.GetParryableNowState())
                {
                    parryable.Parry();
                    anyMatches = true;
                }
            }
            return anyMatches;
        }

        protected override void OnEnter()
        {
            CanDash = true;
        }
        
        protected override void OnUpdate(float deltaTime)
        {

            if (InputManager.GetJumpWasPressedThisFrame())
            {
                Machine.GetState<Root>().JumpBufferTimer = player.locomotionData.jumpBufferTime;
            }
            
            else if (Machine.GetState<Root>().JumpBufferTimer > 0)
            {
                Machine.GetState<Root>().JumpBufferTimer -= deltaTime;
            }
        }
    }

    public class Fall : State
    {
        readonly PlayerCharacterController player;
        private float input;
        private bool isMoving;
    
        public Fall(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
        }

        protected override (State state, string reason) GetNextState()
        {
            if (player.Grounded)
                return (Machine.GetState<Grounded>(), "Player is grounded");
            
            if (InputManager.GetJumpWasPressedThisFrame() && Machine.GetState<Root>().CoyoteTimer > 0)
                return (Machine.GetState<Jump>(), "Player pressed jump while Coyote Timer was active");
            
            return (null, null);
        }

        protected override void OnEnter()
        {
            player.PlayerAnimator.PlayFall();
        }
    
        protected override void OnUpdate(float deltaTime)
        {
            // Gather inputs
            input = InputManager.GetMovement().x;
            isMoving = Mathf.Abs(input) > player.locomotionData.movementInputThreshold;
            if (Machine.GetState<Root>().CoyoteTimer > 0)
            {
                Machine.GetState<Root>().CoyoteTimer -= deltaTime;
            }
        }
    
        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            // Horizontal Movement
            if (isMoving)
            {
                // Flip the player to the correct direction
                bool movingRight = input > 0;
                if (player.isFacingRight != movingRight)
                {
                    player.isFacingRight = movingRight;
                    player.transform.Rotate(0f, movingRight ? 180f : -180f, 0f);
                }

                float targetVelocity = input * player.locomotionData.maxWalkSpeed;
                player.SetHorizontalVelocity(targetVelocity);
            }
            else
            {
                player.SetHorizontalVelocity(0);
            }
        
            // Vertical Movement
            player.IncrementVerticalVelocity(player.locomotionData.Gravity * player.locomotionData.gravityFallMultiplier * fixedDeltaTime);
        }
    }

    public class Jump : State
    {
        readonly PlayerCharacterController player;
        private float apexTimer;
        private float input;
        private bool isMoving;
        private bool jumpCancel;
        private float jumpCancelVelocity;
    
        public Jump(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
        }
    
        protected override (State state, string reason) GetNextState()
        {
            if (player.velocity.y < 0)
                return (Machine.GetState<Fall>(), "Player is moving downward while airborne");
        
            return (null, null);
        }

        protected override void OnEnter()
        {
            player.PlayerAnimator.PlayJump();
            player.SetVerticalVelocity(player.locomotionData.InitialJumpVelocity);            
            apexTimer = 0f;
            jumpCancel = false;
            Machine.GetState<Root>().JumpBufferTimer = 0;
            Machine.GetState<Root>().CoyoteTimer = 0;
        }

        protected override void OnUpdate(float deltaTime)
        {
            // Gather inputs
            input = InputManager.GetMovement().x;
            isMoving = Mathf.Abs(input) > player.locomotionData.movementInputThreshold;
            if (!jumpCancel && !InputManager.GetJumpIsPressed())
            {
                jumpCancel = true;
            }
        }
    
        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            // Horizontal Movement
            if (isMoving)
            {
                // Flip the player to the correct direction
                bool movingRight = input > 0;
                if (player.isFacingRight != movingRight)
                {
                    player.isFacingRight = movingRight;
                    player.transform.Rotate(0f, movingRight ? 180f : -180f, 0f);
                }

                float targetVelocity = input * player.locomotionData.maxWalkSpeed;
                player.SetHorizontalVelocity(targetVelocity);
            }
            else
            {
                player.SetHorizontalVelocity(0);
            }
        
            // Vertical Movement
            // If we bump our head, immediately set ourselves to falling
            if (player.motor.BumpedHead())
            {
                player.SetVerticalVelocity(-0.01f);
            }
        
            // If we canceled our jump, increase the gravity until we're falling
            else if (jumpCancel)
            {
                player.IncrementVerticalVelocity(player.locomotionData.Gravity * player.locomotionData.gravityOnReleaseMultiplier * fixedDeltaTime);
            }
        
            // If we hit the apex of our jump, hang in the air a bit
            else if (player.velocity.y < 0.01f && apexTimer < player.locomotionData.apexHangTime)
            {
                apexTimer += fixedDeltaTime;
                player.SetVerticalVelocity(0);
            }
        
            // Otherwise conform to gravity like normal
            else
            {
                player.IncrementVerticalVelocity(player.locomotionData.Gravity * fixedDeltaTime);
            }
        }
    }
    

    public class JumpParry : State
    {
        readonly PlayerCharacterController player;
        private float apexTimer;
        private float input;
        private bool isMoving;
    
        public JumpParry(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
        }
    
        protected override (State state, string reason) GetNextState()
        {
            if (player.velocity.y < 0)
                return (Machine.GetState<Fall>(), "Player is moving downward while airborne (after Jump-Parry)");
        
            return (null, null);
        }

        protected override void OnEnter()
        {
            apexTimer = 0f;
            Machine.GetState<Airborne>().CanDash = true;
            player.SetVerticalVelocity(player.locomotionData.InitialJumpVelocity);
            player.PlayerAnimator.PlayJumpParry();
        }

        protected override void OnUpdate(float deltaTime)
        {
            // Gather inputs
            input = InputManager.GetMovement().x;
            isMoving = Mathf.Abs(input) > player.locomotionData.movementInputThreshold;

        }
    
        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            // Horizontal Movement
            if (isMoving)
            {
                // Flip the player to the correct direction
                bool movingRight = input > 0;
                if (player.isFacingRight != movingRight)
                {
                    player.isFacingRight = movingRight;
                    player.transform.Rotate(0f, movingRight ? 180f : -180f, 0f);
                }

                float targetVelocity = input * player.locomotionData.maxWalkSpeed;
                player.SetHorizontalVelocity(targetVelocity);
            }
            else
            {
                player.SetHorizontalVelocity(0);
            }
        
            // Vertical Movement
            // If we bump our head, immediately set ourselves to falling
            if (player.motor.BumpedHead())
            {
                player.SetVerticalVelocity(-0.01f);
            }
            // If we hit the apex of our jump, hang in the air a bit
            else if (player.velocity.y < 0.01f && apexTimer < player.locomotionData.apexHangTime)
            {
                apexTimer += fixedDeltaTime;
                player.SetVerticalVelocity(0);
            }
            // Otherwise conform to gravity like normal
            else
            {
                player.IncrementVerticalVelocity(player.locomotionData.Gravity * fixedDeltaTime);
            }
        }
    }

    public class AirDash : State
    {
        readonly PlayerCharacterController player;
        private float dashTimer;
        private float input;
        private bool isMoving;
        
        public AirDash(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
        }

        protected override (State state, string reason) GetNextState()
        {
            if (InputManager.GetSwitchdashWasPressedThisFrame())
                return (Machine.GetState<AirSwitchDash>(), "Player pressed switch-dash");

            if (dashTimer >= player.locomotionData.dashDuration)
                return (Machine.GetState<Fall>(), "Air Dash has ended");
            
            return (null, null);
        }
        
        protected override void OnEnter()
        {
            dashTimer = 0f;
            Machine.GetState<Airborne>().CanDash = false;
            player.PlayerAnimator.PlayAirDash();
        }

        protected override void OnUpdate(float deltaTime)
        {
            // Gather inputs
            input = InputManager.GetMovement().x;
            isMoving = Mathf.Abs(input) > player.locomotionData.movementInputThreshold;

            dashTimer += deltaTime;
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            // Horizontal Movement
            var distance = player.locomotionData.dashDistance;
            var duration = player.locomotionData.dashDuration;
            var finalVelocity = player.locomotionData.maxWalkSpeed;
            var initialVelocity = (2 * distance / duration) - finalVelocity;
            var t = Mathf.Clamp01(dashTimer / duration);
            var velocity = Mathf.Lerp(initialVelocity, finalVelocity, t);
            var direction = player.isFacingRight ? 1f : -1f;
            player.SetHorizontalVelocity(velocity * direction);
            
            // Vertical Movement
            if (dashTimer <= player.locomotionData.airDashHangTime)
                player.SetVerticalVelocity(0f);         
            else
                player.IncrementVerticalVelocity(player.locomotionData.Gravity * player.locomotionData.gravityFallMultiplier * fixedDeltaTime);
        }
    }

        public class AirSwitchDash : State
    {
        readonly PlayerCharacterController player;
        private float switchDashTimer;
        
        public AirSwitchDash(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
        }

        protected override (State state, string reason) GetNextState()
        {
            if (switchDashTimer >= player.locomotionData.switchDashDuration)
                return (Machine.GetState<Idle>(), "Player is done switch-dashing");
    
            return (null, null);
        }
        
        protected override void OnEnter()
        {
            switchDashTimer = 0f;
            player.transform.Rotate(0f, player.isFacingRight ? 180f : -180f, 0f);
            player.isFacingRight = !player.isFacingRight;
            player.PlayerAnimator.PlayAirSwitchDash();
        }

        protected override void OnUpdate(float deltaTime)
        {
            switchDashTimer += deltaTime;
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            // Horizontal Movement
            var distance = player.locomotionData.switchDashDistance;
            var duration = player.locomotionData.switchDashDuration;
            var finalVelocity = player.locomotionData.maxWalkSpeed;
            var initialVelocity = (2 * distance / duration) - finalVelocity;
            var t = Mathf.Clamp01(switchDashTimer / duration);
            var velocity = Mathf.Lerp(initialVelocity, finalVelocity, t);
            var direction = player.isFacingRight ? 1f : -1f;
            player.SetHorizontalVelocity(velocity * direction);
            
            // Vertical Movement
            if (switchDashTimer <= player.locomotionData.airDashHangTime)
                player.SetVerticalVelocity(0f);         
            else
                player.IncrementVerticalVelocity(player.locomotionData.Gravity * player.locomotionData.gravityFallMultiplier * fixedDeltaTime);
        }
    }
}