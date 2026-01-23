using HSM;
using UnityEngine;

namespace Player.States.Locomotion
{
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
            if (InputManager.GetJumpWasPressedThisFrame() || Machine.GetState<Root>().JumpBufferTimer > 0)
            {
                return Machine.GetState<Jump>();
            }

            if (!player.Grounded)
            {
                Machine.GetState<Root>().CoyoteTimer = player.locomotionData.jumpCoyoteTime;
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
}