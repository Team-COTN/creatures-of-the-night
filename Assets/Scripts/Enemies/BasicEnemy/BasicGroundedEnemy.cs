using UnityEngine;

namespace Enemies.BasicEnemy
{
    /// <summary>
    /// Basic ground-based enemy that chases the player (without jumping).
    /// This serves as an example implementation of BasicEnemyStateMachineBase.
    /// 
    /// METHODS YOU CAN OVERRIDE:
    /// - OnIdleStateEnter/Update/FixedUpdate/Exit
    /// - OnChaseStateEnter/Update/FixedUpdate/Exit  
    /// - OnAttackStateEnter/Update/FixedUpdate/Exit
    /// - CanDetectPlayer/CanAttackPlayer for custom detection logic
    /// - Awake/Update/FixedUpdate (ALWAYS call base method first!)
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BasicGroundedEnemy : BasicEnemyStateMachineBase
    {
        [Header("Grounded Enemy Settings")]
        [Tooltip("How fast this enemy moves horizontally when chasing the player")]
        public float movementSpeed = 2f;

        private Rigidbody2D rb;
        private Collider2D col;

        protected override void Awake()
        {
            // Always call base method first. This method sets up the state machine
            base.Awake();

            // Grab the components on our enemy
            
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
        }

        /// <summary>
        /// Put all physics calculations in FixedUpdate functions.
        /// In this case, we're applying gravity to our enemy.
        /// </summary>
        protected override void FixedUpdate()
        {
            // Always call base method first. This method calls the fixed update method of the current state
            base.FixedUpdate(); 

            // Apply gravity when airborne, reset when grounded
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, IsGrounded() ? 0 : rb.linearVelocity.y + Physics2D.gravity.y * Time.deltaTime);
        }

        /// <summary>
        /// Overriding the "OnChaseStateFixedUpdate" function to add the "chasing" behavior.
        /// We do all of our physics logic in "FixedUpdate" instead of "Update" 
        /// </summary>
        protected override void OnFarRangeStateFixedUpdate()
        {
            // Get direction to player
            Vector2 moveDir = Player.position.x >= transform.position.x ? Vector3.right : Vector3.left; 
            
            // Move toward player
            rb.linearVelocity = new Vector2(moveDir.x * movementSpeed, rb.linearVelocity.y); 
        }

        /// <summary>
        /// Overriding the "OnChaseStateExit" to stop moving towards the player
        /// </summary>
        protected override void OnFarRangeStateExit()
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y); // Stop horizontal movement
        }

        /// <summary>
        /// Ground detection using BoxCast checking for any colliders on the "Environment" layer. Hardcoded for simplicity :)
        /// </summary>
        private bool IsGrounded()
        {
            // Cast from bottom of collider
            Vector2 boxCastOrigin = new Vector2(col.bounds.center.x, col.bounds.min.y);
            
            // Thin box matching collider width
            Vector2 boxCastSize = new Vector2(col.bounds.size.x, 0.01f);
            
            // Check for ground
            bool hit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, 0.01f, LayerMask.GetMask("Environment"));
            
            // Return hit result as a boolean
            return hit;
        }

        // EXAMPLE OVERRIDES - Uncomment and modify as needed:
        
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