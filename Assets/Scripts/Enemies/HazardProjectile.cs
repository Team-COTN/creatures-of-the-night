using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class HazardProjectile : MonoBehaviour
{
    [SerializeField] public VisualEffect vfx;
    public float projectileSpeed = 3f;

    public void ReturnToPool() => ServiceLocator.Get<ObjectPooler>().ReturnToPool("HazardProjectile", gameObject);

    void Update() => transform.position += transform.right * projectileSpeed * Time.deltaTime;

    //on wall collision go back in queue (and FX)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<IPlayerShootable>(out IPlayerShootable playerShootable))
        {
            Debug.Log("shot " + playerShootable.GetType().Name);
            playerShootable.TakeShotDamage(1);
            
            ShotFX();            
            ReturnToPool();
        }
    }

    void OnCollisionEnter2D(Collision2D _collider)
    {
        if (_collider.gameObject.layer == LayerMask.NameToLayer("NonComposite"))
        {
            ShotFX();
            ReturnToPool();
        }
    }

    public void ShotFX()
    {
        VisualEffect newVFX = Instantiate(vfx, transform.position, transform.rotation).GetComponent<VisualEffect>();
        newVFX.SendEvent("Hit");
    }

}