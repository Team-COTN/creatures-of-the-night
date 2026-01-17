using System;
using UnityEngine;

public class CharacterProjectileSpawner : MonoBehaviour
{
    private ObjectPooler objectPooler;
    private Character character;
    [SerializeField] private Transform largeProjectileIdleTarget;
    [SerializeField] private Transform smallProjectileIdleTarget;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileReloadTime = 2f;
    private VisualProjectiles largeCharacterProjectile;
    private VisualProjectiles smallCharacterProjectile;
    private float largeProjectileReloadTimer = 0f;
    private float smallProjectileReloadTimer = 0f;
    
    void Awake()
    {
        objectPooler = ServiceLocator.Get<ObjectPooler>();
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
    }
    
    void SpawnNewLargeProjectile()
    {
        largeCharacterProjectile = objectPooler.SpawnFromPool("LargeShard", largeProjectileIdleTarget.position, Quaternion.identity).GetComponent<VisualProjectiles>();
        largeCharacterProjectile.target = largeProjectileIdleTarget;
    }

    void SpawnNewSmallProjectile()
    {
        smallCharacterProjectile = objectPooler.SpawnFromPool("SmallShard", largeProjectileIdleTarget.position, Quaternion.identity).GetComponent<VisualProjectiles>();
        smallCharacterProjectile.target = smallProjectileIdleTarget;
    }
    
    void ShootLargeProjectile()
    {
        Transform projectile = objectPooler.SpawnFromPool("FunctionalProjectile", projectileSpawnPoint.transform.position, Quaternion.identity).transform;
        largeCharacterProjectile.target = projectile;
        largeCharacterProjectile = null;
        largeProjectileReloadTimer = projectileReloadTime;
    }
    
    void ShootSmallProjectile()
    {
        Transform projectile = objectPooler.SpawnFromPool("FunctionalProjectile", projectileSpawnPoint.transform.position, Quaternion.identity).transform;
        smallCharacterProjectile.target = projectile;
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
