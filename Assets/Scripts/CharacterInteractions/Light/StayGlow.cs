using UnityEngine;
using UnityEngine.Rendering.Universal;
using Player;

[RequireComponent(typeof(Light2D))]
public class StayGlow : MonoBehaviour
{ 
    [Header("Intensity Settings")]
    public float brightIntensity; // Max brightness
    public float dimIntensity; // Min brightness
    public float transitionSpeed; // Speed of fading
    public Light2D spotlight;
    private float targetIntensity; // Where the light should move to
    [SerializeField] string vineID;


    public void Awake()
    {
        spotlight = GetComponent<Light2D>();
        spotlight.intensity = dimIntensity;
        float savedIntensity = ES3.Load(SaveKey, dimIntensity);
        targetIntensity = savedIntensity;
    }
    
    private void Update()
    {
        // Smoothly move intensity toward the target
        spotlight.intensity = Mathf.Lerp(spotlight.intensity, targetIntensity, Time.deltaTime * (1 / transitionSpeed));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out PlayerCharacterController playerCharacterController))
        {
            Glow();
        }
    }

    public void Glow()
    {
        targetIntensity = brightIntensity;
        ES3.Save(SaveKey, brightIntensity);
    }

    private string SaveKey => "vineLight_" + vineID;

}