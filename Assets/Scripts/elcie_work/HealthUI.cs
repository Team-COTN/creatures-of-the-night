using UnityEngine;
using Player;

public class HealthUI : MonoBehaviour
{
    //gets access to funcitons declared in the CharacterInteractions class through an instance of that class
    public Animator healthAnimator;
    public Animator dieScreenAnimator;

    [SerializeField] private PlayerCharacterController characterController;

    private void Awake()
    {
        characterController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterController>();
    }
    private void OnEnable()
    {
        characterController.AddCharacterDamagedObserver(UITakeDamage);
    }
    private void OnDisable()
    {
        characterController.RemoveCharacterDamagedObserver(UITakeDamage);
    }

    public void UITakeDamage(int characterHealth)
    {
        if (healthAnimator != null)
            healthAnimator.SetTrigger("Health_Idle");
        
        if (characterHealth == 2)
            healthAnimator.SetTrigger("Damaged1");
        else if (characterHealth == 1)
            healthAnimator.SetTrigger("Damaged2");

        if (characterHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("You died.");
        dieScreenAnimator.SetTrigger("Die");
        Destroy(gameObject);
    }
}
