using DentedPixel;
using HSM;
using UnityEngine;

namespace Player.States.Locomotion
{
    public class Airborne : State
    {
        readonly PlayerCharacterController player;
        public readonly Fall Fall;
        public readonly Jump Jump;
        public readonly AirDash AirDash;
        public bool CanDash;

        protected override State GetDefaultChildState() => Fall;
    
        public Airborne(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
            Fall = new Fall(m, this, player);
            Jump = new Jump(m, this, player);
            AirDash = new AirDash(m, this, player);
        }

        protected override (State state, string reason) GetNextState()
        {
            if (Leaf() == AirDash) return (null, null);
            
            if (InputManager.GetDashWasPressedThisFrame() && CanDash)
                return (AirDash, "Player pressed dash");

            return (null, null);
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
            if (dashTimer >= player.locomotionData.dashDuration)
                return (Machine.GetState<Fall>(), "Air Dash has ended");
            
            return (null, null);
        }
        
        protected override void OnEnter()
        {
            dashTimer = 0f;
            Machine.GetState<Airborne>().CanDash = false;
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
            var t = Mathf.Clamp01(dashTimer / player.locomotionData.dashDuration);
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
}