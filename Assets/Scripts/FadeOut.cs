using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class FadeOut : MonoBehaviour
{
    [Header("Timing")]
    public float delayBeforeFade = 3f;
    public float fadeDuration = 0.2f;
    private bool check = true;
    private HazardProjectile proj;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private BoxCollider2D boxCollider;
    public Light2D childLight;
    private void Awake()
    {
        proj = GetComponent<HazardProjectile>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (proj.GetEnabledStatis() && check)
        {
            StartCoroutine(FadeAndDisableRoutine());
        }
    }

    private IEnumerator FadeAndDisableRoutine()
    {
        check = false; //dont continuously restart the coroutine

        yield return new WaitForSeconds(delayBeforeFade);

        float elapsed = 0f;

        Color spriteColor = spriteRenderer.color;
        float startLightIntensity = childLight != null ? childLight.intensity : 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // Fade sprite alpha
            if (spriteRenderer != null)
            {
                spriteColor.a = Mathf.Lerp(1f, 0f, t);
                spriteRenderer.color = spriteColor;
            }

            // Fade light intensity
            if (childLight != null)
            {
                childLight.intensity = Mathf.Lerp(startLightIntensity, 0f, t);
            }

            yield return null;
        }

        // Ensure fully invisible
        if (spriteRenderer != null)
        {
            spriteColor.a = 0f;
            spriteRenderer.color = spriteColor;
        }

        if (childLight != null)
        {
            childLight.intensity = 0f;
            childLight.enabled = false;
        }

        // Disable colliders
        if (circleCollider != null)
            circleCollider.enabled = false;

        if (boxCollider != null)
            boxCollider.enabled = false;

        if (proj != null)
        {
            //ISS Getting called
            proj.SetFadeStatis(true);

        }

    }

    public void ResetFade()
    {
        Debug.Log("SHOULD make sprite visible again");
        Color spriteColor = spriteRenderer.color;
        spriteColor.a = 1f;
        childLight.intensity = 1f;
        boxCollider.enabled = true;
        circleCollider.enabled = true;
    }
}