using System;
using UnityEngine;
using System.Collections;

//this is a hazard against the player, it will not affect other enemies
public class Hazard : MonoBehaviour
{
    public int hazardStrength;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<ICharacter>() != null)
        {
            if (collision.gameObject.TryGetComponent<IKnockable>(out IKnockable knockable))
            {
                knockable.TakeKnockback((Vector2)transform.position);             
                Debug.Log("knockback");
            }

            if (collision.gameObject.TryGetComponent<IDamagable>(out IDamagable damageable))
                damageable.TakeDamage(hazardStrength); 
        }
    }
}
