using UnityEngine;
using UnityEditor;
using System.Linq;

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
            Machine.Tick(Time.fixedDeltaTime);
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Machine != null)
                Machine.debug = debug;
        }
        
        private void OnDrawGizmos()
        {
            if (!debug) return;
            
            DrawStatePathInfo();
            DrawCustomGizmos();
        }
        
        protected virtual void DrawStatePathInfo()
        {
            var debugInfoPanelText = "";
            
            if (Application.isPlaying)
            {
                var statePath = string.Join(" > ", 
                    Machine.Root.Leaf().PathToRoot().Reverse().Skip(1).Select(n => n.GetType().Name));
                debugInfoPanelText += statePath;
            }
            
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.LowerCenter;
            centeredStyle.normal.textColor = debugTextColor;
            Handles.Label(transform.position + Vector3.up * debugInfoPanelOffset, debugInfoPanelText, centeredStyle);
        }
        
        protected virtual void DrawCustomGizmos() { }
#endif
    }    
}
