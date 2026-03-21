using UnityEngine;


public class SafeGroundCheckpoint : MonoBehaviour
{
   [SerializeField] private LayerMask isCheckpoint;

   public Vector2 safeGroundLocation { get; private set; } = Vector2.zero;
    // ill use this vector in teleport function in SafeGroundDamaged.cs

   public Collider2D playerCol;
   private float safeSpotYOffset;

   private void Start()
   {
       safeGroundLocation = transform.position;
       safeSpotYOffset = playerCol.bounds.size.y / 2;
   }
   void OnTriggerEnter2D(Collider2D collision)
   {
       if ((isCheckpoint.value & (1 << collision.gameObject.layer)) > 0)
       {
           safeGroundLocation = new Vector2(collision.bounds.center.x, collision.bounds.min.y + safeSpotYOffset);
       }
   }

}