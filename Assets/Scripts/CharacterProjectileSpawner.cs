using System;
using UnityEngine;

public class CharacterProjectileSpawner : MonoBehaviour
{
    private ObjectPooler objectPooler;
    [SerializeField] private Transform largeProjectileIdleTarget;
    [SerializeField] private Transform smallProjectileIdleTarget;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileReloadTime = 2f;
    private VisualProjectiles largeCharacterProjectile;
    private VisualProjectiles smallCharacterProjectile;
    private float largeProjectileReloadTimer = 0.1f;
    private float smallProjectileReloadTimer = 0.1f;
    
    void Awake()
    {
        objectPooler = ServiceLocator.Get<ObjectPooler>();
    }
    
    void SpawnNewLargeProjectile()
    {
        largeCharacterProjectile = objectPooler.SpawnFromPool("LargeShard", largeProjectileIdleTarget.position, Quaternion.identity).GetComponent<VisualProjectiles>();
        largeCharacterProjectile.SetTarget(largeProjectileIdleTarget);
    }

    void SpawnNewSmallProjectile()
    {
        smallCharacterProjectile = objectPooler.SpawnFromPool("SmallShard", largeProjectileIdleTarget.position, Quaternion.identity).GetComponent<VisualProjectiles>();
        smallCharacterProjectile.SetTarget(smallProjectileIdleTarget);
    }
    
    void ShootLargeProjectile()
    {
        Transform projectile = objectPooler.SpawnFromPool("FunctionalProjectile", projectileSpawnPoint.transform.position, Quaternion.identity).transform;
        largeCharacterProjectile.SetTarget(projectile);
        largeCharacterProjectile = null;
        largeProjectileReloadTimer = projectileReloadTime;
    }
    
    void ShootSmallProjectile()
    {
        Transform projectile = objectPooler.SpawnFromPool("FunctionalProjectile", projectileSpawnPoint.transform.position, Quaternion.identity).transform;
        smallCharacterProjectile.SetTarget(projectile);
        smallCharacterProjectile = null;
        smallProjectileReloadTimer = projectileReloadTime;
    }
    
    void Update()
    {
        if (largeProjectileReloadTimer > 0)
            largeProjectileReloadTimer -= Time.deltaTime;
        else if (largeCharacterProjectile == null)
            SpawnNewLargeProjectile();
        
        if (smallProjectileReloadTimer > 0)
            smallProjectileReloadTimer -= Time.deltaTime;
        else if (smallCharacterProjectile == null)
            SpawnNewSmallProjectile();
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (largeCharacterProjectile != null)
                ShootLargeProjectile();
            else if (smallCharacterProjectile != null)
                ShootSmallProjectile();
        }
    }
}
