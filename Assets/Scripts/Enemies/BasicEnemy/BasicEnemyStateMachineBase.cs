using UnityEngine;
using UnityEditor;

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
        [SerializeField] private float detectionRange = 5.5f;
        [SerializeField] private bool detectionLosRequired = true;
        [SerializeField] protected LayerMask lineOfSightBlockers;
        [SerializeField] private float attackRange = 2f;
        protected Transform Player;

        protected EnemyStateBase IdleState;
        protected EnemyStateBase ChaseState;
        protected EnemyStateBase AttackState;
        
        protected virtual EnemyStateBase CreateIdleState() => new DefaultIdleState();
        protected virtual EnemyStateBase CreateChaseState() => new DefaultChaseState();
        protected virtual EnemyStateBase CreateAttackState() => new DefaultAttackState();
        
        protected virtual void Awake()
        {
            Player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (Player == null) Debug.LogWarning($"No GameObject with tag 'Player' found. Enemy state transitions may not work.");

            IdleState = CreateIdleState();
            ChaseState = CreateChaseState();
            AttackState = CreateAttackState();
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
        public void ChangeToChase() => ChangeState(ChaseState);
        public void ChangeToAttack() => ChangeState(AttackState);
        
        protected virtual void OnIdleStateEnter() { }
        protected virtual void OnIdleStateUpdate() { }
        protected virtual void OnIdleStateFixedUpdate() { }
        protected virtual void OnIdleStateExit() { }
        
        protected virtual void OnChaseStateEnter() { }
        protected virtual void OnChaseStateUpdate() { }
        protected virtual void OnChaseStateFixedUpdate() { }
        protected virtual void OnChaseStateExit() { }
        
        protected virtual void OnAttackStateEnter() { }
        protected virtual void OnAttackStateUpdate() { }
        protected virtual void OnAttackStateFixedUpdate() { }
        protected virtual void OnAttackStateExit() { }

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
            return canSeePlayer;
        }

        public virtual bool CanAttackPlayer()
        {
            float distanceToPlayer = (!Player) ? float.MaxValue : Vector3.Distance(transform.position, Player.position);
            return distanceToPlayer <= attackRange;
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
                    enemy.ChangeToChase();
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

        private class DefaultChaseState : EnemyStateBase
        {
            public override void Enter(BasicEnemyStateMachineBase enemy)
            {
                enemy.OnChaseStateEnter();
            }

            public override void Update(BasicEnemyStateMachineBase enemy)
            {
                if (enemy.CanAttackPlayer())
                {
                    enemy.ChangeToAttack();
                    return;
                } else if (!enemy.CanDetectPlayer())
                {
                    enemy.ChangeToIdle();
                    return;
                }
                enemy.OnChaseStateUpdate();
            }

            public override void FixedUpdate(BasicEnemyStateMachineBase enemy)
            {
                enemy.OnChaseStateFixedUpdate();
            }

            public override void Exit(BasicEnemyStateMachineBase enemy)
            {
                enemy.OnChaseStateExit();
            }
        }

        private class DefaultAttackState : EnemyStateBase
        {
            public override void Enter(BasicEnemyStateMachineBase enemy)
            {
                enemy.OnAttackStateEnter();
            }

            public override void Update(BasicEnemyStateMachineBase enemy)
            {
                if (!enemy.CanAttackPlayer())
                {
                    enemy.ChangeToChase();
                    return;
                }
                enemy.OnAttackStateUpdate();
            }

            public override void FixedUpdate(BasicEnemyStateMachineBase enemy)
            {
                enemy.OnAttackStateFixedUpdate();
            }

            public override void Exit(BasicEnemyStateMachineBase enemy)
            {
                enemy.OnAttackStateExit();
            }
        }
        
#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            if (!debugVisuals) return;
            
            // Detection Range
            if (currentState == IdleState || currentState == ChaseState)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, detectionRange);
            }

            // Close Range
            if (currentState == ChaseState || currentState == AttackState)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, attackRange);
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