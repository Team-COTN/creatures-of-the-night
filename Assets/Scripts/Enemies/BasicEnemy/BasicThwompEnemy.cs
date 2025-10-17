using System.Collections;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Enemies.BasicEnemy
{



    public class BasicThwompEnemy : BasicEnemyStateMachineBase
    {
        private bool triggerReady = true;

        // EXAMPLE OVERRIDES - Uncomment and modify as needed:
        // If you decide to use Awake, Update, or FixedUpdate, ALWAYS call base method first, like this:
        // protected override void Awake()
        // {
        //     base.Awake();
        //     // Add any custom setup logic here
        // }










        private void OnTriggerEnter2D(Collider2D other)
        {

            Debug.Log("Entered Collision");

            if (other.attachedRigidbody.TryGetComponent(out Character charater) && triggerReady)
            {
                Debug.Log("Character triggered");

                StartCoroutine(Transformation());


            }
        }

        private IEnumerator Transformation()
        {
            triggerReady = false;
            float realTime = 0f;
            float duration = .5f;
            UnityEngine.Vector3 startPos = transform.position;
            UnityEngine.Vector3 destination = startPos + new UnityEngine.Vector3(0f, -6f, 0f);

            while (realTime < duration)
            {
                //float t = Mathf.SmoothStep(0, 1, realTime / duration);
                float t = (realTime / duration) * (realTime / duration);
                transform.position = UnityEngine.Vector3.Lerp(startPos, destination, t);
                realTime += Time.deltaTime;
                yield return null;
            }
       

            yield return new WaitForSeconds(2f);
            realTime = 0f;
            duration = 1.5f;
            startPos = transform.position;
            destination = startPos + new UnityEngine.Vector3(0f, 6f, 0f);

            while (realTime < duration)
            {
                transform.position = UnityEngine.Vector3.Lerp(startPos, destination, realTime / duration);
                realTime += Time.deltaTime;
                yield return null;
            }
            
            triggerReady = true;

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
