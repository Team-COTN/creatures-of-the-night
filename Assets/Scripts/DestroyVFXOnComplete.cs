using UnityEngine;
using UnityEngine.VFX;

public class DestroyVFXOnComplete : MonoBehaviour
{
    void OnEnable()
    {
        // A simple timer-based approach (adjust time as needed)
        Destroy(gameObject, 3f); // Destroys the GameObject after 3 seconds
    }
}