using UnityEngine;

public class HealthUI : MonoBehaviour
{

    public int maxHealth = 100;
    public int currentHealth;
    public Animator Health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (Health != null)
        {
            Health.SetTrigger("Damage1");
        }

        if (currentHealth <= 0)
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
