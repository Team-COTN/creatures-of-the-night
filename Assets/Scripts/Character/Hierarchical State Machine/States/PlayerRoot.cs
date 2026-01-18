using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace HSM
{
    public class PlayerRoot : State
    {
        readonly PlayerCharacterController player;
        public readonly Grounded Grounded;
        public readonly Airborne Airborne;

        public float jumpBufferTimer;
        public float coyoteTimer;

        public PlayerRoot(StateMachine m, PlayerCharacterController player) : base(m, null)
        {
            this.player = player;
            Grounded = new Grounded(m, this, player);
            Airborne = new Airborne(m, this, player);
        }

        protected override State GetDefaultChildState() => Grounded;
        protected override State GetNextState() => null;
    }
    
    public class Grounded : State
    {
        readonly PlayerCharacterController player;
        public readonly Idle Idle; 
        public readonly Move Move;

        public Grounded(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
            Idle = new Idle(m, this, player);
            Move = new Move(m, this, player);
        }

        protected override State GetDefaultChildState() => Idle;

        protected override State GetNextState()
        {
            if (InputManager.GetJumpWasPressedThisFrame() || Machine.GetState<PlayerRoot>().jumpBufferTimer > 0)
            {
                return Machine.GetState<Jump>();
            }

            if (!player.grounded)
            {
                Machine.GetState<PlayerRoot>().coyoteTimer = player.jumpCoyoteTime;
                return Machine.GetState<Airborne>();
            }
            
            return null;
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            player.SetVerticalVelocity(-0.01f);
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
            if (Mathf.Abs(InputManager.GetMovement().x) > 0.1f)
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
            if (Mathf.Abs(InputManager.GetMovement().x) <= 0.1f)
            {
                return Machine.GetState<Idle>();
            }

            return null;
        }

        protected override void OnUpdate(float deltaTime)
        {
            // Gather inputs
            input = InputManager.GetMovement().x;
            isMoving = Mathf.Abs(input) > 0.25f;
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            // Horizontal Movement
            if (isMoving)
            {
                // Flip the player to the correct direction
                bool movingRight = input > 0;
                if (player.IsFacingRight != movingRight)
                {
                    player.IsFacingRight = movingRight;
                    player.transform.Rotate(0f, movingRight ? 180f : -180f, 0f);
                }

                float targetVelocity = input * player.maxWalkSpeed;
                player.SetHorizontalVelocity(targetVelocity);
            }
            else
            {
                player.SetHorizontalVelocity(0);
            }
        }
    }
    
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
                Machine.GetState<PlayerRoot>().jumpBufferTimer = player.jumpBufferTime;
            }
            
            else if (Machine.GetState<PlayerRoot>().jumpBufferTimer > 0)
            {
                Machine.GetState<PlayerRoot>().jumpBufferTimer -= deltaTime;
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
            if (player.grounded)
            {
                return Machine.GetState<Grounded>();
            }

            if (InputManager.GetJumpWasPressedThisFrame() && Machine.GetState<PlayerRoot>().coyoteTimer > 0)
            {
                return Machine.GetState<Jump>();
            }

            return null;
        }
        
        protected override void OnUpdate(float deltaTime)
        {
            // Gather inputs
            input = InputManager.GetMovement().x;
            isMoving = Mathf.Abs(input) > 0.25f;
            if (Machine.GetState<PlayerRoot>().coyoteTimer > 0)
            {
                Machine.GetState<PlayerRoot>().coyoteTimer -= deltaTime;
            }
        }
        
        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            // Horizontal Movement
            if (isMoving)
            {
                // Flip the player to the correct direction
                bool movingRight = input > 0;
                if (player.IsFacingRight != movingRight)
                {
                    player.IsFacingRight = movingRight;
                    player.transform.Rotate(0f, movingRight ? 180f : -180f, 0f);
                }

                float targetVelocity = input * player.maxWalkSpeed;
                player.SetHorizontalVelocity(targetVelocity);
            }
            else
            {
                player.SetHorizontalVelocity(0);
            }
            
            // Vertical Movement
            player.IncrementVerticalVelocity(player.Gravity * 1.25f * fixedDeltaTime);
        }
    }

    public class Jump : State
    {
        readonly PlayerCharacterController player;
        private float apexTimer;
        private float jumpTimer;
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
            player.SetVerticalVelocity(player.InitialJumpVelocity);
            apexTimer = 0f;
            jumpTimer = 0f;
            jumpCancel = false;
            Machine.GetState<PlayerRoot>().jumpBufferTimer = 0;
            Machine.GetState<PlayerRoot>().coyoteTimer = 0;
        }

        protected override void OnUpdate(float deltaTime)
        {
            // Gather inputs
            input = InputManager.GetMovement().x;
            isMoving = Mathf.Abs(input) > 0.25f;
            jumpTimer += deltaTime;
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
                if (player.IsFacingRight != movingRight)
                {
                    player.IsFacingRight = movingRight;
                    player.transform.Rotate(0f, movingRight ? 180f : -180f, 0f);
                }

                float targetVelocity = input * player.maxWalkSpeed;
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
                player.IncrementVerticalVelocity(player.Gravity * 3f * fixedDeltaTime);
            }
            
            // If we hit the apex of our jump, hang in the air a bit
            else if (player.velocity.y < 0.01f && apexTimer < player.apexHangTime)
            {
                apexTimer += fixedDeltaTime;
                player.SetVerticalVelocity(0);
            }
            
            // Otherwise conform to gravity like normal
            else
            {
                player.IncrementVerticalVelocity(player.Gravity * fixedDeltaTime);
            }
        }
    }
}