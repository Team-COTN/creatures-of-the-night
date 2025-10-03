using UnityEngine;

public class ElcieEnemy : MonoBehaviour
{
    //this was made because i didn't want to potentially mess up the enemy script we already had
    //used for TESTING purposes only, COULD be transfered to actual enemy script later (do not mess with since not official)

    public int damageAmount = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnTriEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthUI healthUI = other.GetComponent<HealthUI>();
            if (healthUI != null)
            {
                healthUI.TakeDamage(damageAmount);
            }

        }
    }
}
