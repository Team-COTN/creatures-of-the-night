using UnityEngine;

public class HazardProjectileSpawner : MonoBehaviour
{
    private ObjectPooler objectPooler;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private string projectileTag = "HazardProjectile";
    [SerializeField] private float spawnInterval = 2f;
    public string direction;

    private float spawnTimer;
    void Awake() => objectPooler = ServiceLocator.Get<ObjectPooler>();
    void SpawnProjectile()
    {
        GameObject projectile = objectPooler.SpawnFromPool(projectileTag, spawnPoint.position, spawnPoint.rotation);
        HazardProjectile hazardProjectile = projectile.GetComponent<HazardProjectile>();
        hazardProjectile.SetDirection(direction);
        // hazardProjectile.SetFadeStatis(false);
        // hazardProjectile.Reset();
    }

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