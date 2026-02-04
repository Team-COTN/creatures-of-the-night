using UnityEngine;

namespace Enemies.BasicEnemy
{

    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BasicCaster : BasicEnemyStateMachineBase
    {

        public float moveSpeed;
        private Collider2D col;
        private Rigidbody2D rb;


        private bool retreat = false;
        public Vector3 jumpForce = new Vector2(300, 300);
        private float retreatCooldown = 0f;

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
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();

        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, IsGrounded() ? 0 : rb.linearVelocity.y + Physics2D.gravity.y * Time.deltaTime);

           if (!retreat)
            {
                retreatCooldown = retreatCooldown + Time.deltaTime;
                if (retreatCooldown >= 3f)
                {
                    retreatCooldown = 0f;
                    retreat = true;
                }
            }

        }

        protected override void OnFarRangeStateFixedUpdate()
        {
            Vector2 moveDir = Player.position.x >= transform.position.x ? Vector3.right : Vector3.left;

            rb.linearVelocity = new Vector2(moveDir.x * moveSpeed, rb.linearVelocity.y);
        }

        /*protected override void OnFarRangeStateExit()
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }*/

        protected override void OnCloseRangeFixedUpdate()
        {
            Vector2 moveDir = Player.position.x >= transform.position.x ? Vector3.right : Vector3.left;
            //rb.linearVelocity = new Vector2(moveDir.x * -moveSpeed, rb.linearVelocity.y);
            if (retreat & IsGrounded())
            {
                rb.AddForce(jumpForce);
                retreat = false;
            }
        }

        private bool IsGrounded()
        {
            Vector2 boxCastOrigin = new Vector2(col.bounds.center.x, col.bounds.min.y);

            Vector2 boxCastSize = new Vector2(col.bounds.size.x, 0.01f);

            bool hit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, 0.01f, LayerMask.GetMask("Environment"));

            return hit;
        }
    

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
