using DentedPixel;
using HSM;
using UnityEngine;

namespace Player.States.Locomotion
{
    public class Grounded : State
    {
        readonly PlayerCharacterController player;
        public readonly Idle Idle; 
        public readonly Move Move;
        public readonly Dash Dash;
        public float DashCooldownTimer;
        
        public Grounded(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
            Idle = new Idle(m, this, player);
            Move = new Move(m, this, player);
            Dash = new Dash(m, this, player);
        }

        protected override State GetDefaultChildState() => Idle;

        protected override State GetNextState()
        {
            // If we're dashing, return null
            if (Leaf() == Dash) return null;
            
            // If we're not grounded, fall
            if (!player.Grounded)
            {
                Machine.GetState<Root>().CoyoteTimer = player.locomotionData.jumpCoyoteTime;
                return Machine.GetState<Fall>();
            }
            
            // Jump on button press or if there is some time remaining in the jump buffer
            if (InputManager.GetJumpWasPressedThisFrame() || Machine.GetState<Root>().JumpBufferTimer > 0)
                return Machine.GetState<Jump>();

            // Dash on button press if it's off cooldown
            if (InputManager.GetDashWasPressedThisFrame() && DashCooldownTimer <= 0)
                return Machine.GetState<Dash>();

            return null;
        }

        protected override void OnEnter()
        {
            DashCooldownTimer = 0f;
        }
        
        protected override void OnUpdate(float deltaTime)
        {
            // If we're not dashing, count down the dash cooldown timer
            if (Leaf() != Dash && DashCooldownTimer > 0)
            {
                DashCooldownTimer -= deltaTime;
            }
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            if (player.Grounded)
            {
                player.SetVerticalVelocity(-0.01f);
            }
        }
    }

    public class Idle : State
    {
        readonly PlayerCharacterController player;

        public Idle(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
        }

        protected override State GetNextState()
        {
            if (Mathf.Abs(InputManager.GetMovement().x) > player.locomotionData.movementInputThreshold)
            {
                return Machine.GetState<Move>();
            }
        
            return null;
        }
    
        protected override void OnEnter()
        {
            player.SetHorizontalVelocity(0f);
        }
    }

    public class Move : State
    {
        readonly PlayerCharacterController player;
        private float input;
        private bool isMoving;

        public Move(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
        }

        protected override State GetNextState()
        {
            if (Mathf.Abs(InputManager.GetMovement().x) <= player.locomotionData.movementInputThreshold)
            {
                return Machine.GetState<Idle>();
            }

            return null;
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
        }
    }

    public class Dash : State
    {
        readonly PlayerCharacterController player;
        private float dashTimer;
        private float input;
        private bool isMoving;
        
        public Dash(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
        }

        protected override State GetNextState()
        {
            if (dashTimer >= player.locomotionData.dashDuration)
            {
                return isMoving ? Machine.GetState<Move>() : Machine.GetState<Idle>();
            }
            return null;
        }
        
        protected override void OnEnter()
        {
            dashTimer = 0f;
            Machine.GetState<Grounded>().DashCooldownTimer = player.locomotionData.dashCooldown;
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