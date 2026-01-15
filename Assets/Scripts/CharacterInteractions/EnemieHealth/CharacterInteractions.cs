using UnityEngine;
using System;
using System.Collections; //events 

public class CharacterInteractions : MonoBehaviour, IDamagable, ICharacter
{
    private Color playerColor;
    public Color hitColor;
    [SerializeField] private SpriteRenderer playerSprite;
   
    public int characterHealth = 3;
    public event Action<int> CharacterDamaged;
    public void AddCharacterDamagedObserver(Action<int> observer) { CharacterDamaged += observer; }
    public void RemoveCharacterDamagedObserver(Action<int> observer) { CharacterDamaged -= observer; }
    
    public void Start()
    {
        playerColor = playerSprite.color;
    }

    public void TakeDamage(int damageAmount)
    {
        characterHealth -= damageAmount;

        //invoke the CharacterDamaged event so the UI knows to do its job (play the heart animation)
        CharacterDamaged?.Invoke(characterHealth);
        StartCoroutine(DamagedColor());
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
