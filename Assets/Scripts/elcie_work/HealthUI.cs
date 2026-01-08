using UnityEngine;

public class HealthUI : MonoBehaviour
{
    //gets access to funcitons declared in the CharacterInteractions class through an instance of that class
    public CharacterInteractions characterInteractions;
    public Animator healthAnimator;
    public Animator dieScreenAnimator;
    private void OnEnable()
    {
        characterInteractions.AddCharacterDamagedObserver(TakeDamage);
    }
    private void OnDisable()
    {
        characterInteractions.RemoveCharacterDamagedObserver(TakeDamage);
    }

    public void TakeDamage(int characterHealth)
    {
        if (healthAnimator != null)
        {
            healthAnimator.SetTrigger("Health_Idle");
        }
        
        if (characterHealth == 2)
        {
            healthAnimator.SetTrigger("Damaged1");
        }
        else if (characterHealth == 1)
        {
            healthAnimator.SetTrigger("Damaged2");
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
        dieScreenAnimator.SetTrigger("Die");
        Destroy(gameObject);
    }
}
