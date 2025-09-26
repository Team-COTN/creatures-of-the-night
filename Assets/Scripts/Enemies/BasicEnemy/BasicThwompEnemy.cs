using UnityEngine;

namespace Enemies.BasicEnemy
{


    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BasicThwompEnemy : BasicEnemyStateMachineBase
    {


        // EXAMPLE OVERRIDES - Uncomment and modify as needed:
        // If you decide to use Awake, Update, or FixedUpdate, ALWAYS call base method first, like this:
        // protected override void Awake()
        // {
        //     base.Awake();
        //     // Add any custom setup logic here
        // }

        protected override void Awake()
        {
            base.Awake();

        }

        protected override void OnCloseRangeEnter()
        {

        }

        protected override bool CanDetectPlayer()
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
        



        /*protected override void OnFarRangeStateExit()
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }*/





        // IDLE STATE OVERRIDES:
        // protected override void OnIdleStateEnter() { } // Called once when entering idle
        // protected override void OnIdleStateUpdate() { } // Called every frame during idle  
        // protected override void OnIdleStateFixedUpdate() { } // Called every physics step during idle
        // protected override void OnIdleStateExit() { } // Called once when leaving idle

        // CHASE STATE OVERRIDES:
        // protected override void OnChaseStateEnter() { } // Called once when starting chase
        // protected override void OnChaseStateUpdate() { } // Called every frame during chase
        // protected override void OnChaseStateFixedUpdate() { } // Called every physics step during chase
        // protected override void OnChaseStateExit() { } // Called once when leaving chase

        // ATTACK STATE OVERRIDES:
        // protected override void OnAttackStateEnter() { } // Called once when starting attack
        // protected override void OnAttackStateUpdate() { } // Called every frame during attack
        // protected override void OnAttackStateFixedUpdate() { } // Called every physics step during attack
        // protected override void OnAttackStateExit() { } // Called once when leaving attack

        // PLAYER DETECTION OVERRIDES:
        // protected override bool CanDetectPlayer() { } // Used to determine if the enemy can see/detect the player
        // protected override bool CanAttackPlayer() { } // Used to determine if the enemy is close enough to attack the player      
    }
}
