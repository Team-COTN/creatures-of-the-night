using HSM;
using UnityEngine;
using UnityEngine.Events;

namespace Enemies.Thwomp
{
    [RequireComponent(typeof(PhysicsMotor))]
    public class ThwompEnemy : StateMachineMonoBehaviour
    {
        [Header("Detection")]
        [Tooltip("Player must be within this horizontal width, anywhere below the Thwomp, to trigger a charge.")]
        [SerializeField] private float detectionWidth = 1.5f;
        [SerializeField] private LayerMask playerLayer;

        [Header("Hit Box")]
        [Tooltip("Box checked against playerLayer while charging down and while attacking, to decide hit vs. miss.")]
        [SerializeField] private Vector2 hitBoxSize = new Vector2(1.2f, 0.3f);
        [SerializeField] private Vector2 hitBoxOffset = new Vector2(0f, -0.5f);

        [Header("Movement")]
        [SerializeField] private float chargeSpeed = 14f;
        [SerializeField] private float retractSpeed = 4f;

        [Header("Timing")]
        [SerializeField] private float hitPauseDuration = 0.4f;
        [SerializeField] private float missPauseDuration = 0.2f;

        [Header("Events")]
        [Tooltip("Invoked once, the moment the Thwomp registers a hit on the player. Wire this to your damage/knockback logic.")]
        public PlayerHitEvent OnPlayerHit;

        private PhysicsMotor _motor;
        public PhysicsMotor Motor => _motor ??= GetComponent<PhysicsMotor>();

        public float DetectionWidth => detectionWidth;
        public LayerMask PlayerLayer => playerLayer;
        public Vector2 HitBoxSize => hitBoxSize;
        public Vector2 HitBoxOffset => hitBoxOffset;
        public float ChargeSpeed => chargeSpeed;
        public float RetractSpeed => retractSpeed;
        public float HitPauseDuration => hitPauseDuration;
        public float MissPauseDuration => missPauseDuration;

        protected override State CreateRootState() => new ThwompRoot(null, this);

        // Tall strip extending downward from the Thwomp - "is the player anywhere in my drop lane?"
        public bool IsPlayerBelow()
        {
            const float lookDistance = 100f;
            Vector2 size = new Vector2(detectionWidth, lookDistance);
            Vector2 center = (Vector2)transform.position + Vector2.down * (lookDistance / 2f);
            return Physics2D.OverlapBox(center, size, 0f, playerLayer);
        }

        // Small box at/near the base of the Thwomp - "is the player close enough right now to be hit?"
        public bool IsPlayerInHitBox()
        {
            Vector2 center = (Vector2)transform.position + hitBoxOffset;
            return Physics2D.OverlapBox(center, hitBoxSize, 0f, playerLayer);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
            const float lookDistance = 100f;
            Vector3 detectCenter = transform.position + Vector3.down * (lookDistance / 2f);
            Gizmos.DrawCube(detectCenter, new Vector3(detectionWidth, lookDistance, 0f));

            Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
            Vector3 hitCenter = transform.position + (Vector3)(Vector2)hitBoxOffset;
            Gizmos.DrawCube(hitCenter, new Vector3(hitBoxSize.x, hitBoxSize.y, 0f));
        }
    }

    [System.Serializable]
    public class PlayerHitEvent : UnityEvent<GameObject> { }
}