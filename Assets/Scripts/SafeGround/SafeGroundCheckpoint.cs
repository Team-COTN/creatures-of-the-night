using UnityEngine;


public class SafeGroundCheckpoint : MonoBehaviour
{
   [SerializeField] private LayerMask isCheckpoint;


   public Vector2 safeGroundLocation {get; private set;} = Vector2.zero;


   public Collider2D playerCol;
   private float safeSpotYOffset;
   //-----------script references-----------
   private Character Character;
   private CharacterInteractions CharacterInteractions;


   private void Start()
   {
       Character = GetComponent<Character>();
       CharacterInteractions = GetComponent<CharacterInteractions>();
       safeGroundLocation = transform.position;
       safeSpotYOffset = (playerCol.bounds.size.y / 2);
   }
   void OnTriggerEnter2D(Collider2D collision)
   {
       //if collision gameobject is withing checkpoint laytermask
       if ((isCheckpoint.value & (1 << collision.gameObject.layer)) > 0)
       {
           safeGroundLocation = new Vector2(collision.bounds.center.x, collision.bounds.min.y + safeSpotYOffset);
       }
   }


   public void Update()
   {
       if(CharacterInteractions.characterHealth <= 0)
       {
           WarpPlayer();
       }
   }
   public void WarpPlayer()
   {
       transform.position = safeGroundLocation;
       CharacterInteractions.characterHealth = 3;


   }


}