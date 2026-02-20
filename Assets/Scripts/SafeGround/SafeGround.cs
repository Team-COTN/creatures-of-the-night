using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SafeGround : MonoBehaviour
{
   [SerializeField] private float saveFrequency = 3f;


   public Vector2 safeGroundLocation {get; private set;} = new Vector2(0f, 0f);


   private Character Character;
   private CharacterInteractions CharacterInteractions;


   private Coroutine coroutine;


   private void Start()
   {
       Character = GetComponent<Character>();
       CharacterInteractions = GetComponent<CharacterInteractions>();
       coroutine = StartCoroutine(SaveGroundLocation());
       safeGroundLocation = transform.position;
   }


   private IEnumerator SaveGroundLocation()
   {
       float elapsedTime = 0f;


       while (elapsedTime < saveFrequency)
       {
           elapsedTime += Time.deltaTime;
           yield return null;
       }


       //saves new position every 3 secs
       if (Character.IsGrounded)
       {
           safeGroundLocation = transform.position;


       }


       coroutine = StartCoroutine(SaveGroundLocation());
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
