using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HSM
{
    public abstract class StateMachineMonoBehaviour : MonoBehaviour
    {
        [Header("State Machine Properties")] 
        [SerializeField] private bool debug = false;
        [SerializeField] private Color debugTextColor = Color.white;
        [SerializeField, Tooltip("Vertical offset for state path display")] 
        private float debugInfoPanelOffset = -0.25f;
        
        public StateMachine Machine { get; private set; }
        protected abstract State CreateRootState();
        
        private void Awake()
        {
            var root = CreateRootState();
            if (root == null)
            {
                Debug.LogError($"CreateRootState() returned null in {GetType().Name}", this);
                enabled = false;
                return;
            }
            var builder = new StateMachineBuilder(root);
            Machine = builder.Build();
            Machine.debug = debug;
        }

        private void Update()
        {
            Machine.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            Machine.FixedTick(Time.fixedDeltaTime);
        }
        
#if UNITY_EDITOR
        private State editorRootState;
        
        private void OnDrawGizmos()
        {
            if (!debug) return;

            // If the application is playing, draw the gizmos of the active states and print the path
            if (Application.isPlaying)
            {
                var debugInfoPanelText = "";
                var states = Machine.ActiveStates.ToList();
                
                // Draw Gizmos for each active state
                foreach (var state in states)
                    state.DrawCustomGizmos();
                
                // Build state path (skipping root)
                debugInfoPanelText = string.Join(" > ", 
                    states.Skip(1).Select(s => s.GetType().Name));
                
                // Print the state path
                GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
                centeredStyle.alignment = TextAnchor.LowerCenter;
                centeredStyle.normal.textColor = debugTextColor;
                Handles.Label(transform.position + Vector3.up * debugInfoPanelOffset, 
                    debugInfoPanelText, centeredStyle);
            }
            
            // If the application isn't playing, draw the gizmos of the default states
            else
            {
                editorRootState ??= CreateRootState();
                State current = editorRootState;
                while (current != null)
                {
                    current.DrawCustomGizmos();
                    current = current.GetDefaultChildState();
                }
            }
        }
        
        private void OnValidate()
        {
            editorRootState = null;
            if (Machine != null)
                Machine.debug = debug;
        }
#endif
    }    
}