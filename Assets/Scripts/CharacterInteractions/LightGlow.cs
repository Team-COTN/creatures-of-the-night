using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightGlow : MonoBehaviour
{ 
    [Header("Intensity Settings")]
    public float brightIntensity; // Max brightness
    public float dimIntensity; // Min brightness
    public float transitionSpeed; // Speed of fading
    public Light2D spotlight;
    private float targetIntensity; // Where the light should move to

    public void Awake()
    {
        spotlight = GetComponent<Light2D>();
        spotlight.intensity = dimIntensity;
        targetIntensity = spotlight.intensity;
    }
    
    private void Update()
    {
        // Smoothly move intensity toward the target
        spotlight.intensity = Mathf.Lerp(spotlight.intensity, targetIntensity, Time.deltaTime * transitionSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("omthing entered");
        
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out Character character))
        {
            Debug.Log("Character entered");
            Glow();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out Character character))
        {
            Dim();
        }
    }
    
    private void Dim() => targetIntensity = dimIntensity;   
    private void Glow() => targetIntensity = brightIntensity;

}
