using System;
using UnityEngine;
using System.Collections;

public class Hazard : MonoBehaviour
{
    public int hazardStrength = 2;
    
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<IDamagable>(out IDamagable damageable))
            damageable.TakeDamage(hazardStrength);
    }
}
