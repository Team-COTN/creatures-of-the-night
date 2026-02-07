using UnityEngine;
using HSM;

namespace Player.Eye.States
{
    // Root state for the eye state machine hierarchy
    public class EyeRoot : State
    {
        readonly EyeController eye;
        public readonly Inactive Inactive;
        public readonly Active Active;

        public EyeRoot(StateMachine m, EyeController eye) : base(m, null)
        {
            this.eye = eye;
            Inactive = new Inactive(m, this, eye);
            Active = new Active(m, this, eye);
        }

        protected override State GetDefaultChildState() => Inactive;

        protected override void OnUpdate(float deltaTime)
        {
            var lightRadius = Mathf.MoveTowards(eye.LightRadius, eye.TargetLightRadius,
                eye.lightTransitionSpeed * deltaTime);
            eye.LightRadius = lightRadius; 
        }
    }

    // Eye state when inactive - follows a transform with no collision
    public class Inactive : State
    {
        readonly EyeController eye;
        public readonly Following Following;
        
        public Inactive(StateMachine m, State parent, EyeController eye) : base(m, parent)
        {
            this.eye = eye;
            Following = new Following(m, this, eye);
        }
        
        protected override State GetDefaultChildState() => Following;
        
        protected override (State state, string reason) GetNextState()
        {
            // If the eye is activated, set state to Active
            if (eye.EyeActive)
                return (Machine.GetState<Active>(), "Eye set to active");
            
            return (null, null);
        }
        
        protected override void OnEnter()
        {
            // Dim Light
            eye.DimLight();
            
            // Disable collisions while inactive
            eye.Motor.CollisionMask = 0;
        }

    }

    // Eye state when active - player controlled with collision enabled
    public class Active : State
    {
        readonly EyeController eye;
        public readonly Scrying Scrying;
        
        public Active(StateMachine m, State parent, EyeController eye) : base(m, parent)
        {
            this.eye = eye;
            Scrying = new Scrying(m, this, eye);
        }
        
        protected override State GetDefaultChildState() => Scrying;

        protected override (State state, string reason) GetNextState()
        {
            // If the eye is deactivated, set state to Inactive
            if (!eye.EyeActive)
                return (Machine.GetState<Inactive>(), "Eye set to inactive");
            
            return (null, null);
        }

        protected override void OnEnter()
        {
            // Brighten Light
            eye.BrightenLight();
            
            // Enable saved collision mask while active
            eye.Motor.CollisionMask = eye.DefaultCollisionMask;
        }
    }
    
    // Passive following behavior when eye is inactive
    public class Following : State
    {
        readonly EyeController eye;

        public Following(StateMachine m, State parent, EyeController eye) : base(m, parent)
        {
            this.eye = eye;
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            // Move towards eye follow transform with distance-based damping
            var position = (Vector2)eye.transform.position;
            var targetPosition = (Vector2)eye.EyeFollowTransform.position;
            var offset = targetPosition - position;
            var velocity = offset * eye.eyeSpeed;

            // Cap max speed
            if (velocity.magnitude > eye.eyeSpeed)
                velocity = velocity.normalized * eye.eyeSpeed;
            
            eye.SetVelocity(velocity);
        }
    }

    // Player-controlled eye movement with tether mechanics
    public class Scrying : State
    {
        readonly EyeController eye;
        private Vector2 input;
        public Scrying(StateMachine m, State parent, EyeController eye) : base(m, parent)
        {
            this.eye = eye;            
        }

        protected override void OnUpdate(float deltaTime)
        {
            // Gather Inputs
            input = InputManager.GetMovement();
            
            // Toggle Ricochet
            if (InputManager.GetIlluminetRicochetWasPressedThisFrame())
                eye.ToggleRicochet();
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            // Eye Movement (tethered to eye follow transform)
            var position = (Vector2)eye.transform.position;
            var targetPosition = (Vector2)eye.EyeFollowTransform.position;
            var offset = targetPosition - position;

            // Determine the magnitude and normalized direction of the difference
            float distanceToCenter = offset.magnitude;
            Vector2 directionToCenterNormalized = offset.normalized;

            // Set speed multiplier based on distance from the player to the eye
            float speedMultiplierByDistance = 1f - Mathf.InverseLerp(eye.circleRangeMin, eye.circleRangeMax, distanceToCenter);

            // Determine speed multiplier based on the direction of movement towards or away from the center
            float speedMultiplierByDirection = Mathf.Clamp(Vector2.Dot(input, directionToCenterNormalized), 0, 1);

            // Choose the larger multiplier between distance and direction-based multipliers
            float speedMultiplier = Mathf.Max(speedMultiplierByDistance, speedMultiplierByDirection);

            // Move the eye using player input, eye speed, and the determined speed multiplier
            eye.SetVelocity(input * (eye.eyeSpeed * speedMultiplier));
        }

        protected override void OnExit()
        {
            // Disable ricochet when exiting scrying state
            eye.DeactivateRicochet();
        }
    }
}