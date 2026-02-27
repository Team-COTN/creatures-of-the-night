using UnityEngine;
using UnityEngine.VFX;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class HazardProjectile : MonoBehaviour
{
    [SerializeField] public VisualEffect vfx;
    public float projectileSpeed = 3f;
    public float secondsUntilFade = 3f;
    public float secondsUntilDisable = 5f;
    public float timer;
    private string direction;
    private bool faded = false;
    private bool hazardEnabled = false;
    // public FadeOut fade;

    public MMF_Player projectileFade;
    public MMF_Player projectileReset;

    public void ReturnToPool() => ServiceLocator.Get<ObjectPooler>().ReturnToPool("HazardProjectile", gameObject);
    public void SetDirection(string d) { direction = d; }
    // public void SetFadeStatis(bool f) { faded = f; }

    // public bool GetEnabledStatis() { return hazardEnabled; }

    // public bool GetFadeStatis() { return faded; }

    private void OnEnable()
    {
        // hazardEnabled = true;
        // fade.ResetFade();
        projectileReset.PlayFeedbacks(); 
        timer = 0;
    }

    // private void OnDisable()
    // {
    //     hazardEnabled = false;
    // }


    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= secondsUntilFade)
        {
            projectileFade.PlayFeedbacks(); 
        }

        if (timer >= secondsUntilDisable)
        {
            ReturnToPool();
        }

        if (direction == "right")
            transform.position += transform.right * projectileSpeed * Time.deltaTime;
        else if (direction == "left")
            transform.position -= transform.right * projectileSpeed * Time.deltaTime;
        else if (direction == "up")
            transform.position += transform.up * projectileSpeed * Time.deltaTime;
        else if (direction == "down")
            transform.position -= transform.up * projectileSpeed * Time.deltaTime;
        else
            Debug.Log("Unknown direction?.");

//ISSS getting called
        // if (faded)
        // {
        //     ReturnToPool();
        // }
    }

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

    //on wall collision go back in queue (and FX)
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

    // public void Reset() 
    // {
    //     fade.ResetFade(); 
    // }

}