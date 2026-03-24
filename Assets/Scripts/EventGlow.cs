using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EventGlow : MonoBehaviour
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

    public void Glow() => targetIntensity = brightIntensity;

}
