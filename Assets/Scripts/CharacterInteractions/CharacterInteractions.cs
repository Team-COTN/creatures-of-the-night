using UnityEngine;
using System;
using System.Collections; //events 
using Player;

public class CharacterInteractions : MonoBehaviour, IPlayerDamagable, ICharacter
{
    private Color playerColor;
    public Color hitColor;
    [SerializeField] private SpriteRenderer playerSprite;

    [SerializeField] private PlayerCharacterController characterController;
    public int characterHealth = 3;
    public event Action<int> CharacterDamaged;
    public event Action<int> DamagedState;

    public void AddCharacterDamagedObserver(Action<int> observer) { CharacterDamaged += observer; }
    public void RemoveCharacterDamagedObserver(Action<int> observer) { CharacterDamaged -= observer; }
    
    private void Awake()
    {
        // characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterController>();
        playerColor = playerSprite.color;
    }


    public void PlayerTakeDamage(int damageAmount)
    {
        characterHealth -= damageAmount;
        characterController.Damage();
        //invoke the CharacterDamaged event so the UI knows to do its job (play the heart animation)
        CharacterDamaged?.Invoke(characterHealth);
        StartCoroutine(DamagedColor());
    }

    public void EnterDamage(Vector2 hazardPosition)
    {
        characterController._hazardPosition = hazardPosition;
    }
    
    IEnumerator DamagedColor()
    {
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
