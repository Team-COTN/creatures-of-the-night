using UnityEngine;

public class LightGlow : MonoBehaviour
{
    [Header("References")]
public Light spotlight; // Assign your spotlight in the inspector

[Header("Intensity Settings")]
public float brightIntensity = 5f; // Max brightness
public float dimIntensity = 1f; // Min brightness
public float transitionSpeed = 2f; // Speed of fading

private float targetIntensity; // Where the light should move to

public void Start()
{
if (spotlight == null)
spotlight = GetComponentInChildren<Light>();

// Start bright by default
spotlight.intensity = brightIntensity;
targetIntensity = brightIntensity;
}

private void Update()
{
// Smoothly move intensity toward the target
spotlight.intensity = Mathf.Lerp(
spotlight.intensity,
targetIntensity,
Time.deltaTime * transitionSpeed
);
}

private void OnTriggerEnter(Collider other)
{
if (other.CompareTag("Player"))
{
// When player enters → dim the light
targetIntensity = dimIntensity;
}
}

private void OnTriggerExit(Collider other)
{
if (other.CompareTag("Player"))
{
// When player exits → return to bright
targetIntensity = brightIntensity;
}
}
}
