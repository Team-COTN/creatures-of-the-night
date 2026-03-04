using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering.Universal;
using System.Collections;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class FadeHazardProjectile : MonoBehaviour
{
    [SerializeField] public VisualEffect vfx;

    [Header("Movement")]
    public float projectileSpeed = 3f;
    private string direction;

    [Header("Fade Settings")]
    public float fadeDuration = 0.2f;

    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private BoxCollider2D boxCollider;
    private Light2D childLight;

    private float originalLightIntensity;

    public void ReturnToPool() =>
        ServiceLocator.Get<ObjectPooler>().ReturnToPool("HazardProjectile", gameObject);

    public void SetDirection(string d)
    {
        ResetProjectile();
        direction = d;
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        childLight = GetComponentInChildren<Light2D>();

        if (childLight != null)
            originalLightIntensity = childLight.intensity;
    }

    void Update()
    {
        if (direction == "right")
            transform.position += transform.right * projectileSpeed * Time.deltaTime;
        else if (direction == "left")
            transform.position -= transform.right * projectileSpeed * Time.deltaTime;
        else if (direction == "up")
            transform.position += transform.up * projectileSpeed * Time.deltaTime;
        else if (direction == "down")
            transform.position -= transform.up * projectileSpeed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<IPlayerShootable>(out IPlayerShootable playerShootable))
        {
            playerShootable.TakeShotDamage(1);
            ShotFX();
            StartCoroutine(FadeAndReturn());
        }
    }

    void OnCollisionEnter2D(Collision2D _collider)
    {
        if (_collider.gameObject.layer == LayerMask.NameToLayer("NonComposite"))
        {
            ShotFX();
            StartCoroutine(FadeAndReturn());
        }
    }

    IEnumerator FadeAndReturn()
    {
        // Disable colliders immediately to prevent double hits
        circleCollider.enabled = false;
        if (boxCollider != null)
            boxCollider.enabled = false;

        float elapsed = 0f;
        Color color = spriteRenderer.color;
        float startLightIntensity = childLight != null ? childLight.intensity : 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // Fade sprite
            color.a = Mathf.Lerp(1f, 0f, t);
            spriteRenderer.color = color;

            // Fade light
            if (childLight != null)
                childLight.intensity = Mathf.Lerp(startLightIntensity, 0f, t);

            yield return null;
        }

        ReturnToPool();
    }

    void ResetProjectile()
    {
        // Reset sprite alpha
        Color color = spriteRenderer.color;
        color.a = 1f;
        spriteRenderer.color = color;

        // Reset colliders
        circleCollider.enabled = true;
        if (boxCollider != null)
            boxCollider.enabled = true;

        // Reset light
        if (childLight != null)
        {
            childLight.enabled = true;
            childLight.intensity = originalLightIntensity;
        }
    }

    public void ShotFX()
    {
        VisualEffect newVFX = Instantiate(vfx, transform.position, transform.rotation)
            .GetComponent<VisualEffect>();
        newVFX.SendEvent("Hit");
    }
}