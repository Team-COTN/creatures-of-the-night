using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace HSM
{
    public class PlayerCharacterController : MonoBehaviour
    {
        [Header("References")]
        public PhysicsMotor motor;
        public StateMachine Machine;
        private State root;
        public Vector2 velocity;
        public bool isFacingRight = true;

        [Header("Settings")]
        public CharacterLocomotionData locomotionData;
        
        [Header("Debug")]
        public bool debugInfoPanel = true;
        public float debugInfoPanelHeight = 10f;
        
        private void Awake()
        {
            root = new PlayerRoot(null, this);
            var builder = new StateMachineBuilder(root);
            Machine = builder.Build();
            
            Machine.OnStateEntered<Jump>(OnPlayerJump);
        }
        
        private void Update()
        {
            Machine.Tick(Time.deltaTime);
        }
        
        private void FixedUpdate()
        {
            Machine.FixedTick(Time.fixedDeltaTime);
            motor.Move(velocity * Time.fixedDeltaTime);
        }
        
        public bool Grounded => motor.IsGrounded();
        
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

        public void OnPlayerJump()
        {
            Debug.Log("Jump");
        }
        
        
#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            if (!debugInfoPanel) return;

            // Create Info Panel text
            var debugInfoPanelText = "";
            
            // Add State name to Debug Info
            if (Application.isPlaying)
            {
                var statePath = string.Join(" > ", Machine.Root.Leaf().PathToRoot().Reverse().Skip(1).Select(n => n.GetType().Name));
                debugInfoPanelText += statePath;
            }
            
            // Draw Info Panel
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.LowerCenter;
            Handles.Label(transform.position + Vector3.up * debugInfoPanelHeight, debugInfoPanelText, centeredStyle);
        }
#endif
    }
}