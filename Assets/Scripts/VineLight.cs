using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
[RequireComponent(typeof(Collider2D))]
public class VineLight : MonoBehaviour
{
    [SerializeField] Light2D vinePointLight;
    [SerializeField] string vineID;

    void Start()
    {
            float savedRadius = ES3.Load(SaveKey, .2f);
            // ES3.Load("pointLightOuterRadius", .2f);
            vinePointLight.pointLightOuterRadius = savedRadius;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody.TryGetComponent(out Character character))
        {
            vinePointLight.pointLightOuterRadius = 3f;
            ES3.Save(SaveKey, 3f);
        }
    }
    private string SaveKey => "vineLight_" + vineID;
}
