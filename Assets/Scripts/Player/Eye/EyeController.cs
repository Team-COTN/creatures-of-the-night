using System.Linq;
using UnityEngine;
using UnityEditor;
using HSM;
using Player.Eye.States;

namespace Player.Eye
{
    public class EyeController : MonoBehaviour
    {
        [Header("References")]
        public PhysicsMotor motor;
        public StateMachine Machine;
        private State root;
        public Transform eyeFollowTransform;
        
        [Header("Settings")]
        [HideInInspector] public Vector2 velocity;
        [HideInInspector] public bool eyeActive;
        public float eyeSpeed = 10f;
        public float circleRangeMin = 8f;
        public float circleRangeMax = 10f;

        [Header("Debug")] 
        public bool debugInfoPanel = true;
        public float debugInfoPanelHeight = 10f;
        
        private void Awake()
        {
            root = new EyeRoot(null, this);
            var builder = new StateMachineBuilder(root);
            Machine = builder.Build();
            Machine.debug = true;
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

        public void SetVelocity(Vector2 value) => velocity = value;
        public void ActivateEye() => eyeActive = true;
        public void DeactivateEye() => eyeActive = false;
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
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

            // Draw Gizmos for specific states
            if (Application.isPlaying && Machine.debug)
            {
                // Draw a colored line based on distance from idle position
                var position = (Vector2)transform.position;
                var targetPosition = (Vector2)eyeFollowTransform.position;
                var offset = targetPosition - position;
                var c = Mathf.Lerp(0f, 0.3f, 1 - Mathf.Clamp01(offset.magnitude / circleRangeMax));
                Debug.DrawLine(transform.position, eyeFollowTransform.position, Color.HSVToRGB(c, 1, 1));
                
                // Scrying
                if (Machine.Root.Leaf() == Machine.GetState<Scrying>())
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(eyeFollowTransform.position, circleRangeMin);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(eyeFollowTransform.position, circleRangeMax);
                }
            }
        }
#endif
    }
}