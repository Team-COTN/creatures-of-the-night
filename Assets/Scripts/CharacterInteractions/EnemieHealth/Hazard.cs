using System;
using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour
{
    public int hazardStrength;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<IPlayerDamagable>(out IPlayerDamagable playerDamageable))
        {
            playerDamageable.PlayerTakeDamage(hazardStrength);
            //give player the Hazard position
            playerDamageable.EnterDamage((Vector2)transform.position); 
        }
    }
}
