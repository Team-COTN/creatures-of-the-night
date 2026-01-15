using System;
using UnityEngine;

public class CharacterProjectileSpawner : MonoBehaviour
{
    private ObjectPooler objectPooler;
    [SerializeField] private GameObject largeCharacterProjectile;
    [SerializeField] private GameObject smallCharacterProjectile;
    [SerializeField] private GameObject projectileTarget;
    
    void Awake()
    {
        objectPooler = ServiceLocator.Get<ObjectPooler>();
    }
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            objectPooler.SpawnFromPool("largeCharacterProjectile", projectileTarget.transform.position, Quaternion.identity);
            objectPooler.SpawnFromPool("largeCharacterProjectile", projectileTarget.transform.position, Quaternion.identity);

        }
    }
    
    
}
