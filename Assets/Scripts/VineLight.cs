using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class VineLight : MonoBehaviour
{
    [SerializeField] Light2D vinePointLight;
    //[SerializeField] GameObject vinePointLight;

    // Start is called before the first frame update
    void Start()
    {
            vinePointLight.pointLightOuterRadius = 2;
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter2D(Collider2D other)
    {
        /*
        Debug.Log("Entered Collision");
        if (other.TryGetComponent(out Character character))
        {
            Debug.Log("Character triggered");
        }
        */
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered Player Collision");
            vinePointLight.pointLightOuterRadius = 25;
        }
        
    }
}
