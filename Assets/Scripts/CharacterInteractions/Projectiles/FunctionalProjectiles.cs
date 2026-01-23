using UnityEngine;
using UnityEngine.VFX;

//attatched to GO. WITH colider
[RequireComponent(typeof(CircleCollider2D))]

//may make a different script replacing this for ricochet OR
//just adjust functionality depending on abilities unlocked
public class FunctionalProjectiles : MonoBehaviour
{
    [SerializeField] public VisualEffect vfx;
    private float projectileSpeed = 3f;
    private bool enabled = false;
    public VisualProjectiles visual;
    
    //gets instantiated on slash attack (button click)
    //A Shard will need to be assigned a projectile. The Large Shard should always go 1st and only enabled shards should be assigned. BUT can have more than 2 shards in total enabled

    private void OnEnable() => enabled = true;

    private void OnDisable() => enabled = false;

    public void SetVisual(VisualProjectiles visual) => this.visual = visual;
    public void ReturnToPool() => ServiceLocator.Get<ObjectPooler>().ReturnToPool("FunctionalProjectile", gameObject);
    
    void Update()
    {
        if (enabled)
        {
            //wait until attack animation done? May or may not need bc transition speed
            transform.position += transform.right * projectileSpeed * Time.deltaTime;
        }
    }
    
    //on wall collision go back in queue (and FX)
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.TryGetComponent<IShootable>(out IShootable shootable))
        {
            Debug.Log("shot " + shootable.GetType().Name);
            shootable.TakeShotDamage(1);

            vfx.SendEvent("Hit");

            if (visual)
            {
                visual.ReturnToPool();
            }
            ReturnToPool();
        }
    }
}