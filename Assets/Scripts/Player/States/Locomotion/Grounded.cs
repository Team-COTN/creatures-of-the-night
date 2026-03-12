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
        public readonly SwitchDash SwitchDash;
        public readonly Slash Slash;

        public float DashCooldownTimer;
        [SerializeField] CharacterInteractions characterInteractions;
        

        public Grounded(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
            Idle = new Idle(m, this, player);
            Move = new Move(m, this, player);
            Dash = new Dash(m, this, player);
            SwitchDash = new SwitchDash(m, this, player);
            Slash = new Slash(m, this, player);
        }

        protected override State GetDefaultChildState() => Mathf.Abs(InputManager.GetMovement().x) > player.locomotionData.movementInputThreshold ? Move : Idle;

        protected override (State state, string reason) GetNextState()
        {
            // If we're dashing, return null
            if (Leaf() == Dash || Leaf() == SwitchDash) return (null, null);
            
            // If we're not grounded, fall
            if (!player.Grounded)
            {
                Machine.GetState<Root>().CoyoteTimer = player.locomotionData.jumpCoyoteTime;
                return (Machine.GetState<Fall>(), "Player not grounded");
            }
            
            // Control the eye on button press
            if (InputManager.GetEyeWasPressedThisFrame())
                return (Machine.GetState<Scrying>(), "Player pressed eye button");
            
            // Jump on button press
            if (InputManager.GetJumpWasPressedThisFrame()) 
                return (Machine.GetState<Jump>(), "Player pressed jump");
            
            // Jump if the jump buffer had time remaining
            if (Machine.GetState<Root>().JumpBufferTimer > 0)
                return (Machine.GetState<Jump>(), "Jump buffer has time remaining");

            // Dash on button press if it's off cooldown
            if (InputManager.GetDashWasPressedThisFrame() && DashCooldownTimer <= 0)
                return (Machine.GetState<Dash>(), "Player pressed dash");
            
            // Slash attack on button press
            if (InputManager.GetSlashWasPressedThisFrame())
                return (Machine.GetState<Slash>(), "Player pressed slash attack");

            return (null, null);
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

        protected override (State state, string reason) GetNextState()
        {
            if (Mathf.Abs(InputManager.GetMovement().x) > player.locomotionData.movementInputThreshold)
                return (Machine.GetState<Move>(), "Player is pressing movement buttons");
            
            return (null, null);
        }
    
        protected override void OnEnter()
        {
            player.PlayerAnimator.PlayIdle();
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

        protected override (State state, string reason) GetNextState()
        {
            if (Mathf.Abs(InputManager.GetMovement().x) <= player.locomotionData.movementInputThreshold)
                return (Machine.GetState<Idle>(), "Player is not pressing movement buttons");
            
            return (null, null);
        }

        protected override void OnEnter()
        {
            player.PlayerAnimator.PlayWalk();
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

        protected override (State state, string reason) GetNextState()
        {
            if (InputManager.GetSwitchdashWasPressedThisFrame())
                return (Machine.GetState<SwitchDash>(), "Player pressed switch-dash");

            if (dashTimer >= player.locomotionData.dashDuration)
                return (Machine.GetState<Idle>(), "Player is done dashing");
    
            return (null, null);
        }
        
        protected override void OnEnter()
        {
            dashTimer = 0f;
            Machine.GetState<Grounded>().DashCooldownTimer = player.locomotionData.dashCooldown;
            player.PlayerAnimator.PlayDash();
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

    public class SwitchDash : State
    {
        readonly PlayerCharacterController player;
        private float switchDashTimer;
        
        public SwitchDash(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
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
            player.PlayerAnimator.PlaySwitchDash();
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
    
    public class Slash : State
    {
        readonly PlayerCharacterController player;
        private float slashTimer;
        
        public Slash(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
        }

        protected override (State state, string reason) GetNextState()
        {
            if (slashTimer >= player.locomotionData.slashDuration)
            { 
                return (Machine.GetState<Idle>(), "Player finished grounded slash attack");
            }
    
            return (null, null);
        }

        protected override void OnEnter()
        {
            player.PlayerAnimator.PlaySlash();
        }

        protected override void OnUpdate(float deltaTime)
        {
            //make damage collider
            float radius = .5f;
        
            //need to find if the player has a weapon in hand
            //if no weapon, the arm of the character is the weapon
            Vector2 weaponColOrigin = player.attackCollider2D.bounds.center;
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

            slashTimer += deltaTime;
        }
        
    }
}
