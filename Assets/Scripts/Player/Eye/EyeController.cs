using System.Linq;
using UnityEngine;
using UnityEditor;
using HSM;
using Player.Eye.States;
using UnityEngine.Rendering.Universal;

namespace Player.Eye
{
    [RequireComponent(typeof(PhysicsMotor))]
    public class EyeController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PhysicsMotor motor;
        [SerializeField] private Transform eyeFollowTransform;
        [SerializeField] private Collider2D ricochetCollider;
        [SerializeField] private Light2D eyePointLight;

        [Header("Settings")]
        public float eyeSpeed = 10f;
        public float circleRangeMin = 8f;
        public float circleRangeMax = 10f;
        public float minLightRadius = 2.5f;
        public float maxLightRadius = 7f;
        public float lightTransitionSpeed = 1f;
        
        [Header("Debug")] 
        public bool debug = true;
        public float debugInfoPanelHeight = 10f;
        
        // Properties
        public PhysicsMotor Motor => motor;
        public Transform EyeFollowTransform => eyeFollowTransform;
        public float LightRadius
        {
            get => eyePointLight.pointLightOuterRadius;
            set => eyePointLight.pointLightOuterRadius = value;
        }
        public float TargetLightRadius => targetLightRadius;
        public bool EyeActive => eyeActive;
        public LayerMask DefaultCollisionMask => defaultCollisionMask;
        
        // Private variables
        private Vector2 velocity;
        private int defaultCollisionMask;
        private bool eyeActive;
        private float targetLightRadius;
        public StateMachine Machine;
        private State root;
        
        private void Awake()
        {
            defaultCollisionMask = motor.CollisionMask;
            root = new EyeRoot(null, this);
            var builder = new StateMachineBuilder(root);
            Machine = builder.Build();
        }

        private void Start()
        {
            targetLightRadius = minLightRadius;
            eyePointLight.pointLightOuterRadius = targetLightRadius;
        }
        
        private void Update()
        {
            Machine.Tick(Time.deltaTime);
            Machine.debug = debug;
        }

        private void FixedUpdate()
        {
            Machine.FixedTick(Time.fixedDeltaTime);
            Motor.Move(velocity * Time.fixedDeltaTime);
        }

        public void SetVelocity(Vector2 value) => velocity = value;
        public void ActivateEye() => eyeActive = true;
        public void DeactivateEye() => eyeActive = false;
        public void ActivateRicochet() => ricochetCollider.enabled = true;
        public void DeactivateRicochet() => ricochetCollider.enabled = false;
        public void ToggleRicochet() => ricochetCollider.enabled = !ricochetCollider.enabled;
        public void DimLight() => targetLightRadius = minLightRadius;
        public void BrightenLight() => targetLightRadius = maxLightRadius;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!debug) return;

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
                var targetPosition = (Vector2)EyeFollowTransform.position;
                var offset = targetPosition - position;
                var c = Mathf.Lerp(0f, 0.3f, 1 - Mathf.Clamp01(offset.magnitude / circleRangeMax));
                Debug.DrawLine(transform.position, EyeFollowTransform.position, Color.HSVToRGB(c, 1, 1));
                
                // Scrying
                if (Machine.Root.Leaf() == Machine.GetState<Scrying>())
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(EyeFollowTransform.position, circleRangeMin);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(EyeFollowTransform.position, circleRangeMax);
                }
            }
        }
#endif
    }
}