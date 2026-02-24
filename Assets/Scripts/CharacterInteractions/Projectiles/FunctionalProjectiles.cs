using UnityEngine;
using UnityEngine.VFX;
using Player;
// using Player.Data;

//attatched to GO. WITH colider
[RequireComponent(typeof(CircleCollider2D))]

//may make a different script replacing this for ricochet OR
//just adjust functionality depending on abilities unlocked
public class FunctionalProjectiles : MonoBehaviour
{
    [SerializeField] public VisualEffect vfx;
    private PlayerCharacterController character;

    public float projectileSpeed = 3f;
    private bool enabled = false;
    private bool direction;

    public VisualProjectiles visual;
    

    //gets instantiated on slash attack (button click)
    //A Shard will need to be assigned a projectile. The Large Shard should always go 1st and only enabled shards should be assigned. BUT can have more than 2 shards in total enabled

    private void Awake() => character = GameObject.FindWithTag("Player").GetComponent<PlayerCharacterController>();

    private void OnEnable()
    {
        enabled = true;
        direction = character.isFacingRight;
    }

    private void OnDisable() => enabled = false;

    public void SetVisual(VisualProjectiles visual) => this.visual = visual;
    public void ReturnToPool() => ServiceLocator.Get<ObjectPooler>().ReturnToPool("FunctionalProjectile", gameObject);
    
    void Update()
    {
        if (enabled)
        {
            //wait until attack animation done? May or may not need bc transition speed
            if (direction)
                transform.position += transform.right * projectileSpeed * Time.deltaTime;
            else
                transform.position += transform.right * projectileSpeed * Time.deltaTime * -1f;
        }
    }
    
    //on wall collision go back in queue (and FX)
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.TryGetComponent<IShootable>(out IShootable shootable))
        {
            Debug.Log("shot " + shootable.GetType().Name);
            shootable.TakeShotDamage(1);
            
            VisualEffect newVFX = Instantiate(vfx, transform.position, transform.rotation).GetComponent<VisualEffect>();
            newVFX.SendEvent("Hit");
            
            if (visual)
            {
                visual.ReturnToPool();
            }
            ReturnToPool();
        }
    }
}
