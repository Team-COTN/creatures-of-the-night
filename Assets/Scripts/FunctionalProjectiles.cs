using UnityEngine;

//attatched to GO. WITH colider
[RequireComponent(typeof(CircleCollider2D))]

//may make a different script replacing this for ricochet OR
//just adjust functionality depending on abilities unlocked
public class FunctionalProjectiles : MonoBehaviour
{
    private Character character;
    public float projectileSpeed;
    public float Xoffset;
    public float Yoffset;
    private Vector2 offset;


    //gets instantiated on slash attack (button click)
    private void OnEnable()
    {
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        transform.position = character.transform.position += (Vector3)(new Vector2(Xoffset, Yoffset));
    }
    
    void Update()
    {
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }
    
    //on wall colision go back in queue
}
