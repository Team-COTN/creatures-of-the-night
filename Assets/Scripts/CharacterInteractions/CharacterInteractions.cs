using UnityEngine;
using System; //events 

public class CharacterInteractions : MonoBehaviour, IDamagable
{
    //may need to make private later
    public int characterHealth = 3;
    public event Action<int> CharacterDamaged;
    public void AddCharacterDamagedObserver(Action<int> observer) { CharacterDamaged += observer; }
    public void RemoveCharacterDamagedObserver(Action<int> observer) { CharacterDamaged -= observer; }

    //TakeDamage() will be called from the hazard script which inflicts the damage
    
    public void TakeDamage(int damageAmount)
    {
        Debug.Log("Before Health: " + characterHealth);
        characterHealth -= damageAmount;
        Debug.Log("NOW Health: " + characterHealth);

        //invoke the CharacterDamaged event so the UI knows to do its job (play the heart animation)
        CharacterDamaged?.Invoke(characterHealth);
    }

        //if a colision happens
        //get the damageAmount 
        //damage the player by damageAmount
}
