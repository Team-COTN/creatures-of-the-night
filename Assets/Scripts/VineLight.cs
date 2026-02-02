using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
[RequireComponent(typeof(Collider2D))]
public class VineLight : MonoBehaviour
{
    [SerializeField] Light2D vinePointLight;

    void Start()
    {
            vinePointLight.pointLightOuterRadius = .2f;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody.TryGetComponent(out Character character))
        {
            vinePointLight.pointLightOuterRadius = 3f;
        }
    }

}
