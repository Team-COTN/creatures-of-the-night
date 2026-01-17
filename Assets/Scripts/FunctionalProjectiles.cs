using UnityEngine;

//attatched to GO. WITH colider
[RequireComponent(typeof(CircleCollider2D))]

//may make a different script replacing this for ricochet OR
//just adjust functionality depending on abilities unlocked
public class FunctionalProjectiles : MonoBehaviour
{
    public float projectileSpeed;
    private bool enabledVar = false;


    //gets instantiated on slash attack (button click)
    //A Shard will need to be assigned a projectile. The Large Shard should always go 1st and only enabled shards should be assigned. BUT can have more than 2 shards in total enabled

    private void OnEnable() { enabledVar = true; }
    private void OnDisable() { enabledVar = false; }

    void Update()
    {
        if (enabledVar)
        {
            //wait until attack animation done? May or may not need bc transition speed
            transform.position += transform.right * projectileSpeed * Time.deltaTime;
        }
    }
    
    //on wall colision go back in queue (and FX)
}
