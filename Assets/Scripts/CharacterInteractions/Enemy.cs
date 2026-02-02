using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    private int enemyStrength = 1;
    void Update()
    {
        //enemy behaviour
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("collision Occured");
        IDamagable damagable = col.collider.GetComponent<IDamagable>();
        if (damagable != null)
            damagable.TakeDamage(enemyStrength);
    }

    public void TakeDamage(int damageAmount)
    {
        Debug.Log("OOF by:" + damageAmount);
    }
}
