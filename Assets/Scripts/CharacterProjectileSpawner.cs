using UnityEngine;

public class CharacterProjectileSpawner : MonoBehaviour
{
    private ObjectPooler objectPooler;
    [SerializeField] private GameObject largeCharacterProjectile;

    void Awake()
    {
        objectPooler = ServiceLocator.Get<ObjectPooler>();
    }
    void FixedUpdate()
    {
        objectPooler.SpawnFromPool("largeCharacterProjectile", transform.position, Quaternion.identity);
    }
    
    
}
