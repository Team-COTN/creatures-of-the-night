using UnityEngine;

public class HazardProjectileSpawner : MonoBehaviour
{
    private ObjectPooler objectPooler;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private string projectileTag = "HazardProjectile";
    [SerializeField] private float spawnInterval = 2f;
    private float spawnTimer;
    void Awake() => objectPooler = ServiceLocator.Get<ObjectPooler>();
    void SpawnProjectile() => objectPooler.SpawnFromPool(projectileTag, spawnPoint.position, spawnPoint.rotation);

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnProjectile();
            spawnTimer = spawnInterval;
        }
    }
}