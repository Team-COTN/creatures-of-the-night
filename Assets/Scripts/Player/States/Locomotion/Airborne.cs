using HSM;
using UnityEngine;

namespace Player.States.Locomotion
{
    public class Airborne : State
    {
        readonly PlayerCharacterController player;
        public readonly Fall Fall;
        public readonly Jump Jump;

        protected override State GetDefaultChildState() => Fall;
    
        public Airborne(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
            this.Fall = new Fall(m, this, player);
            this.Jump = new Jump(m, this, player);
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

        protected override State GetNextState()
        {
            if (player.Grounded)
            {
                return Machine.GetState<Grounded>();
            }

            if (InputManager.GetJumpWasPressedThisFrame() && Machine.GetState<Root>().CoyoteTimer > 0)
            {
                return Machine.GetState<Jump>();
            }

            return null;
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
    
        protected override State GetNextState()
        {
            if (player.velocity.y < 0)
            {
                return Machine.GetState<Fall>();
            }
        
            return null;
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
}