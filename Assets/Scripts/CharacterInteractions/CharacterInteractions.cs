using UnityEngine;
using System;
using System.Collections; //events 

public class CharacterInteractions : MonoBehaviour, IDamagable, ICharacter
{
    private Character character;
    private bool facingRight;

    public Vector2 KBForce = new Vector2(10f, 10f);
    public Vector2 KBForceBack = new Vector2(-10f, 10f);
    private Color playerColor;
    public Color hitColor;
    [SerializeField] private SpriteRenderer playerSprite;
   
    public int characterHealth = 3;
    public event Action<int> CharacterDamaged;
    public void AddCharacterDamagedObserver(Action<int> observer) { CharacterDamaged += observer; }
    public void RemoveCharacterDamagedObserver(Action<int> observer) { CharacterDamaged -= observer; }
    
    //TakeDamage() will be called from the hazard script which inflicts the damage

    public void Start()
    {
        character = GameObject.FindWithTag("Player").GetComponent<Character>();
        playerColor = playerSprite.color;
    }
    public void Update()
    {
        //facingRight = character.IsFacingRight;
    }
    public void TakeDamage(int damageAmount)
    {
        Debug.Log("Before Health: " + characterHealth);
        characterHealth -= damageAmount;
        Debug.Log("NOW Health: " + characterHealth);

        //invoke the CharacterDamaged event so the UI knows to do its job (play the heart animation)
        CharacterDamaged?.Invoke(characterHealth);

        //make player temporarily turn red
        //make player take knockback (apply a force in the direction the player was hit)
        // if (facingRight)
        // {
        //     character.rb.AddForce(KBForce, ForceMode2D.Impulse);
        //     Debug.Log("is getting knocked back while facing right");
        // }
        // else
        // {
        //     character.rb.AddForce(KBForceBack, ForceMode2D.Impulse);
        // }
        StartCoroutine(DamagedColor());

    }

    //if a colision happens
    //get the damageAmount 
    //damage the player by damageAmount

    IEnumerator DamagedColor()
    {
        Debug.Log("coroutine on");

        float duration = 2f;
        float realTime = 0f;
        

        while (realTime < duration)
        {
            playerSprite.color = Color.Lerp(hitColor, playerColor, realTime / duration);
            realTime += Time.deltaTime;
            yield return null;
        }

    }
}
