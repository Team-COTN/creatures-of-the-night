using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class StayGlow : MonoBehaviour
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
        spotlight.intensity = Mathf.Lerp(spotlight.intensity, targetIntensity, Time.deltaTime * (1 / transitionSpeed));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out Character character))
        {
            Glow();
        }
    }

    public void Glow() => targetIntensity = brightIntensity;

}