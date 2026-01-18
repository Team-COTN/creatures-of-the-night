using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace HSM
{
    public class PlayerCharacterController : MonoBehaviour
    {
        public PhysicsMotor motor;
        private StateMachine machine;
        private State root;
        public Vector2 velocity;
        private string lastPath;
        public bool IsFacingRight = true;
        public bool debugInfoPanel = true;
        public float debugInfoPanelHeight = 10f;
        
        [Header("Walking")]
        [Range(0f, 1f)] public float moveThreshold = 0.25f;
        [Range(1f, 100f)] public float maxWalkSpeed = 12.5f;
        [Range(0.25f, 50f)] public float groundAcceleration = 5f;
        [Range(0.25f, 50f)] public float groundDeceleration = 20f;

        [Header("Dashing")]
        public float dashDuration = 0.5f;
        public float dashDistance = 1.0f;
        public float dashCooldown = 1f;
        public float switchdashDuration = .2f;
        public float switchdashDistance = 3.5f;
        public float switchdashCooldown = 0.15f;

        
        [Header("Jumping and Falling")]
        public float jumpHeight = 6.5f;
        public float timeTillJumpApex = 0.35f;
        public float timeToJumpCancel = 0.25f;
        [Range(0.01f, 5f)] public float gravityOnReleaseMultiplier = 2f;
        [Range(0.5f, 1f)] public float apexThreshold = 0.97f;
        [Range(0.01f, 1f)] public float apexHangTime = 0.075f;
        
        [Header("Jump Buffer and Coyote Time")]
        [Range(0f, 1f)] public float jumpBufferTime = 0.125f;
        [Range(0f, 1f)] public float jumpCoyoteTime = 0.1f;
        
        [Header("Air Movement")]
        [Range(0.25f, 50f)] public float airHorizontalAcceleration = 5f;
        [Range(0.25f, 50f)] public float airHorizontalDeceleration = 20f;
        
        private const float JumpHeightCompensationFactor = 1.054f;
        public float Gravity { get; private set; }
        public float InitialJumpVelocity { get; private set; }
        private float AdjustedJumpHeight { get; set; }
        
        private void OnValidate() => CalculateJumpStats();
        private void OnEnable() => CalculateJumpStats();
        private void CalculateJumpStats()
        {
            AdjustedJumpHeight = jumpHeight * JumpHeightCompensationFactor;
            Gravity = -(2f * AdjustedJumpHeight) / Mathf.Pow(timeTillJumpApex, 2f);
            InitialJumpVelocity = Mathf.Abs(Gravity) * timeTillJumpApex;
        }
        
        private void Awake()
        {
            root = new PlayerRoot(null, this);
            var builder = new StateMachineBuilder(root);
            machine = builder.Build();
        }
        
        private void Update()
        {
            machine.Tick(Time.deltaTime);
        }
        
        private void FixedUpdate()
        {
            machine.FixedTick(Time.fixedDeltaTime);
            motor.Move(velocity * Time.fixedDeltaTime);
        }
        
        public void SetVerticalVelocity(float value)
        {
            velocity = new Vector2(velocity.x, value);
        }

        public void IncrementVerticalVelocity(float value)
        {
            velocity += new Vector2(0, value);
        }

        public void SetHorizontalVelocity(float value)
        {
            velocity = new Vector2(value, velocity.y);
        }

        public void IncrementHorizontalVelocity(float value)
        {
            velocity += new Vector2(value, 0);
        }
        
        public bool grounded => motor.IsGrounded();
        
#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            if (!debugInfoPanel) return;

            // Create Info Panel text
            var debugInfoPanelText = "";
            
            // Add State name to Debug Info
            if (Application.isPlaying)
            {
                var statePath = string.Join(" > ", machine.Root.Leaf().PathToRoot().Reverse().Skip(1).Select(n => n.GetType().Name));
                debugInfoPanelText += statePath ?? "NULL ERROR";
            }
            
            // Draw Info Panel
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.LowerCenter;
            Handles.Label(transform.position + Vector3.up * debugInfoPanelHeight, debugInfoPanelText, centeredStyle);
        }
#endif
    }
}