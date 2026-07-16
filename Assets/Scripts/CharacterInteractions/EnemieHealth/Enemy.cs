using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable, IShootable, IBlockable
{
    // public int enemyStrength = 1;
    public int enemyHP = 5;
    public int damageTaken = 0;

    void Update()
    {
        if (enemyHP <= damageTaken)
            Destroy(this.gameObject);

    }

    // public void OnCollisionEnter2D(Collision2D col)
    // {
    //     Debug.Log("collision Occured");
    //     IDamagable damagable = col.collider.GetComponent<IDamagable>();
    //     if (damagable != null)
    //         damagable.TakeDamage(enemyStrength);
    // }

    public void TakeShotDamage(int damageAmount)
    {
        damageTaken+= damageAmount;
        Debug.Log("OOF Shot by:" + damageAmount);
        Debug.Log("Total Damage Taken: " + damageTaken);

    }

    public void TakeDamage(int damageAmount)
    {
        damageTaken+= damageAmount;
        Debug.Log("OOF by:" + damageAmount);
    }

    public void GetBlocked()
    {
        Debug.Log("I've been blocked!");
    }

}
