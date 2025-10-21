using UnityEngine;

public class HealthUI : MonoBehaviour
{
    //gets access to functions declared in the CharacterInteractions class through an instance of that class
    private CharacterInteractions characterInteractions;
    public Animator healthAnimator;
    private void OnEnable()
    {
        characterInteractions.AddCharacterDamagedObserver(TakeDamage);
    }
    private void OnDisable()
    {
        characterInteractions.RemoveCharacterDamagedObserver(TakeDamage);
    }

    // maxHealth is already defined and updated in CharacterInteractions
    /*
    void Start()
    {
        currentHealth = maxHealth;
    }
    */

    public void TakeDamage(int characterHealth)
    {
        if (healthAnimator != null)
        {
            healthAnimator.SetTrigger("Damage1");
        }

        //the characterhealth is already defined in the CharacterInteractions class.
        //when CharacterDamaged is invoked, we pass in the "currentHealth" as an argument
        if (characterHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("You died.");
        Destroy(gameObject);
    }
}
