using UnityEngine;
using UnityEditor;
using TMPro;

namespace Enemies.BasicEnemy
{
    public abstract class EnemyStateBase
    {
        public abstract void Enter(BasicEnemyStateMachineBase enemyStateMachine);
        public abstract void Update(BasicEnemyStateMachineBase enemyStateMachine);
        public abstract void FixedUpdate(BasicEnemyStateMachineBase enemyStateMachine);
        public abstract void Exit(BasicEnemyStateMachineBase enemyStateMachine);
    }

    public abstract class BasicEnemyStateMachineBase : MonoBehaviour
    {
        protected EnemyStateBase currentState;

        [Header("Debug")] 
        [SerializeField] private bool debugInfoPanel = true;
        [SerializeField] private float debugInfoPanelHeight = 1.15f;

        [Space]
        [Tooltip("Visuals will only appear when enemy is selected in the hierarchy")]
        [SerializeField] private bool debugVisuals = true;
        [SerializeField] private bool debugLogStateChanges = true;
        
        [Header("Basic Enemy Settings")]
        [SerializeField] protected float detectionRange = 5.5f;
        [SerializeField] protected bool detectionLosRequired = true;
        [SerializeField] protected LayerMask lineOfSightBlockers;
        [SerializeField] private float closeRange = 2f;
        protected Transform Player;

        protected Rigidbody2D rb;
        private bool isKnockedBack = false;
        protected float kbForce = 200f;
        protected float kbDuration = 0.3f;

        protected EnemyStateBase IdleState;
        protected EnemyStateBase FarRangeState;//chase chase
        protected EnemyStateBase CloseRangeState;//attack attack
        
        protected virtual EnemyStateBase CreateIdleState() => new DefaultIdleState();
        protected virtual EnemyStateBase CreateFarRangeState() => new DefaultFarRangeState();
        protected virtual EnemyStateBase CreateCloseRangeState() => new DefaultCloseRangeState();
        
        protected virtual void Awake()
        {
            Player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (Player == null) Debug.LogWarning($"No GameObject with tag 'Player' found. Enemy state transitions may not work.");

            rb = GetComponent<Rigidbody2D>();


            IdleState = CreateIdleState();
            FarRangeState = CreateFarRangeState();
            CloseRangeState = CreateCloseRangeState();
            ChangeState(IdleState);
        }
        
        protected virtual void Update()
        {
            currentState?.Update(this);
        }

        protected virtual void FixedUpdate()
        {
            currentState?.FixedUpdate(this);
        }

        private void ChangeState(EnemyStateBase newState)
        {
#if UNITY_EDITOR
            if (debugLogStateChanges) Debug.Log($"{name} State Change: {currentState?.GetType().Name ?? "null"} -> {newState?.GetType().Name ?? "null"}");
#endif
            currentState?.Exit(this);
            currentState = newState;
            currentState?.Enter(this);
        }
        
        public void ChangeToIdle() => ChangeState(IdleState);
        public void ChangeToFarRange() => ChangeState(FarRangeState);
        public void ChangeToCloseRange() => ChangeState(CloseRangeState);
        
        protected virtual void OnIdleStateEnter() { }
        protected virtual void OnIdleStateUpdate() { }
        protected virtual void OnIdleStateFixedUpdate() { }
        protected virtual void OnIdleStateExit() { }
        
        protected virtual void OnFarRangeStateEnter() { }
        protected virtual void OnFarRangeStateUpdate() { }
        protected virtual void OnFarRangeStateFixedUpdate() { }
        protected virtual void OnFarRangeStateExit() { }
        
        protected virtual void OnCloseRangeEnter() { }
        protected virtual void OnCloseRangeUpdate() { }
        protected virtual void OnCloseRangeFixedUpdate() { }
        protected virtual void OnCloseRangeExit() { }

        protected virtual bool CanDetectPlayer()
        {
            if (!Player) return false;

            var distanceToPlayer = Vector3.Distance(transform.position, Player.position);
            if (distanceToPlayer > detectionRange) return false;
            if (!detectionLosRequired) return true;
    
            var directionToPlayer = Player.position - transform.position;
            var hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, lineOfSightBlockers);
    
            // If nothing blocks our line of sight, or our raycast hits our player, then we can see the player
            bool canSeePlayer = !hit || hit.collider?.attachedRigidbody?.transform == Player;

            GetComponent<Rigidbody2D>().AddForce(Vector2.right);
            return canSeePlayer;
        }

        public virtual bool CanAttackPlayer()
        {
            float distanceToPlayer = (!Player) ? float.MaxValue : Vector3.Distance(transform.position, Player.position);
            return distanceToPlayer <= closeRange;
        }

        
        
        private class DefaultIdleState : EnemyStateBase
        {
            public override void Enter(BasicEnemyStateMachineBase enemy)
            {
                enemy.OnIdleStateEnter();
            }

            public override void Update(BasicEnemyStateMachineBase enemy)
            {
                if (enemy.CanDetectPlayer())
                {
                    enemy.ChangeToFarRange();
                    return;
                }
                enemy.OnIdleStateUpdate();
            }

            public override void FixedUpdate(BasicEnemyStateMachineBase enemy)
            {
                enemy.OnIdleStateFixedUpdate();
            }

            public override void Exit(BasicEnemyStateMachineBase enemy)
            {
                enemy.OnIdleStateExit();
            }
        }

        private class DefaultFarRangeState : EnemyStateBase
        {
            public override void Enter(BasicEnemyStateMachineBase enemy)
            {
                enemy.OnFarRangeStateEnter();
            }

            public override void Update(BasicEnemyStateMachineBase enemy)
            {
                if (enemy.CanAttackPlayer())
                {
                    enemy.ChangeToCloseRange();
                    return;
                } else if (!enemy.CanDetectPlayer())
                {
                    enemy.ChangeToIdle();
                    return;
                }
                enemy.OnFarRangeStateUpdate();
            }

            public override void FixedUpdate(BasicEnemyStateMachineBase enemy)
            {
                enemy.OnFarRangeStateFixedUpdate();
            }

            public override void Exit(BasicEnemyStateMachineBase enemy)
            {
                enemy.OnFarRangeStateExit();
            }
        }

        private class DefaultCloseRangeState : EnemyStateBase
        {
            public override void Enter(BasicEnemyStateMachineBase enemy)
            {
                enemy.OnCloseRangeEnter();
            }

            public override void Update(BasicEnemyStateMachineBase enemy)
            {
                if (!enemy.CanAttackPlayer())
                {
                    enemy.ChangeToFarRange();
                    return;
                }
                enemy.OnCloseRangeUpdate();
            }

            public override void FixedUpdate(BasicEnemyStateMachineBase enemy)
            {
                enemy.OnCloseRangeFixedUpdate();
            }

            public override void Exit(BasicEnemyStateMachineBase enemy)
            {
                enemy.OnCloseRangeExit();
            }
        }
        
#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            if (!debugVisuals) return;
            
            // Detection Range
            if (currentState == IdleState || currentState == FarRangeState)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, detectionRange);
            }

            // Close Range
            if (currentState == FarRangeState || currentState == CloseRangeState)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, closeRange);
            }
            
            // Line of Sight
            if (Player != null && detectionLosRequired)
            {
                var distanceToPlayer = Vector3.Distance(transform.position, Player.position);
                if (distanceToPlayer < detectionRange)
                {
                    var directionToPlayer = Player.position - transform.position;
                    var hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, lineOfSightBlockers);
        
                    // If nothing blocks our line of sight, or our raycast hits our player, then we can see the player
                    bool canSeePlayer = !hit || hit.collider?.attachedRigidbody?.transform == Player;
                    Gizmos.color = canSeePlayer ? Color.red : Color.yellow;
        
                    Vector3 endPoint = hit.collider ? (Vector3)hit.point : Player.position;
                    Gizmos.DrawLine(transform.position, endPoint);
                }
            }
        }

        public virtual void EnemyDamaged()
        {
            if (Player != null && !isKnockedBack)
            {
                Vector2 kbDirection = (transform.position - Player.position).normalized;

                rb.AddForce(kbDirection * kbForce, ForceMode2D.Impulse);

               //StartCoroutine(KBCoroutine());
            }
        }

       /* private System.Collections.IEnumerator KBCoroutine()
        {
            isKnockedBack = true;

            yield return new WaitForSeconds(kbDuration);

            isKnockedBack = false;

            rb.linearVelocity = Vector2.zero;

            currentState?.FixedUpdate(this);

        }*/

        protected virtual void OnDrawGizmos()
        {
            if (!debugInfoPanel) return;

            // Create Info Panel text
            var debugInfoPanelText = "";
            
            // Add State name to Debug Info
            debugInfoPanelText += currentState?.GetType().Name ?? "null";
            
            // Draw Info Panel
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.LowerCenter;
            Handles.Label(transform.position + Vector3.up * debugInfoPanelHeight, debugInfoPanelText, centeredStyle);
        }
#endif
    }
}