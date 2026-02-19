using UnityEngine;

/// <summary>
/// Smoothly and randomly changes the opacity AND color of a sprite over time.
/// Attach this script to any GameObject with a SpriteRenderer.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Mist : MonoBehaviour
{
    [Header("Opacity Range (0 = fully transparent, 1 = fully opaque)")]
    [Range(0f, 1f)] public float minAlpha = 0.2f;
    [Range(0f, 1f)] public float maxAlpha = 1f;

    [Header("Alpha Speed Settings")]
    public float minChangeDuration = 1f;
    public float maxChangeDuration = 3f;

    [Header("Color Change Speed")]
    public float minColorChangeSpeed = 0.5f;
    public float maxColorChangeSpeed = 2f;

    private SpriteRenderer spriteRenderer;

    // Alpha variables
    private float targetAlpha;
    private float changeDuration;
    private float elapsedTime;
    private float startAlpha;

    // Color variables
    private Color targetColor;
    private float colorChangeSpeed;

    private readonly Color[] palette = new Color[]
    {
        new Color(0.6f, 0.8f, 1f), // Light Blue
        new Color(0.8f, 0.6f, 1f), // Purple
        Color.white
    };

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (minAlpha > maxAlpha)
        {
            float temp = minAlpha;
            minAlpha = maxAlpha;
            maxAlpha = temp;
        }
    }

    void Start()
    {
        startAlpha = spriteRenderer.color.a;
        targetColor = palette[Random.Range(0, palette.Length)];
        colorChangeSpeed = Random.Range(minColorChangeSpeed, maxColorChangeSpeed);

        PickNewAlphaTarget();
    }

    void Update()
    {
        UpdateAlpha();
        UpdateColor();
    }

    void UpdateAlpha()
    {
        elapsedTime += Time.deltaTime;

        float t = Mathf.Clamp01(elapsedTime / changeDuration);
        float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

        Color c = spriteRenderer.color;
        c.a = newAlpha;
        spriteRenderer.color = c;

        if (t >= 1f)
        {
            startAlpha = targetAlpha;
            PickNewAlphaTarget();
        }
    }

    void UpdateColor()
    {
        Color currentColor = spriteRenderer.color;

        // Preserve current alpha while changing RGB
        Color targetWithCurrentAlpha = new Color(
            targetColor.r,
            targetColor.g,
            targetColor.b,
            currentColor.a
        );

        Color newColor = Color.Lerp(
            currentColor,
            targetWithCurrentAlpha,
            Time.deltaTime * colorChangeSpeed
        );

        spriteRenderer.color = newColor;

        // If close to target, pick new color
        if (Vector3.Distance(
            new Vector3(newColor.r, newColor.g, newColor.b),
            new Vector3(targetColor.r, targetColor.g, targetColor.b)) < 0.02f)
        {
            targetColor = palette[Random.Range(0, palette.Length)];
            colorChangeSpeed = Random.Range(minColorChangeSpeed, maxColorChangeSpeed);
        }
    }

    private void PickNewAlphaTarget()
    {
        targetAlpha = Random.Range(minAlpha, maxAlpha);
        changeDuration = Random.Range(minChangeDuration, maxChangeDuration);
        elapsedTime = 0f;
    }
}

/*
using UnityEngine;

/// <summary>
/// Smoothly and randomly changes a sprite's opacity and hue between
/// light blue, purple, and white.
/// Attach this script to any GameObject with a SpriteRenderer.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Mist : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    // Target color and alpha
    private Color targetColor;
    private float targetAlpha;

    // Transition speed
    [SerializeField] private float colorChangeSpeed = 0.5f; // lower = slower
    [SerializeField] private float alphaChangeSpeed = 0.5f;

    // Possible colors to blend between
    private readonly Color[] palette = new Color[]
    {
        new Color(0.6f, 0.8f, 1f), // Light Blue
        new Color(0.8f, 0.6f, 1f), // Purple
        Color.white                 // White
    };

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize with a random color and alpha
        targetColor = palette[Random.Range(0, palette.Length)];
        targetAlpha = Random.Range(0.3f, 1f); // Avoid fully invisible
    }

    private void Update()
    {
        // Smoothly interpolate towards target color
        Color currentColor = spriteRenderer.color;
        Color newColor = Color.Lerp(currentColor, new Color(targetColor.r, targetColor.g, targetColor.b, currentColor.a), Time.deltaTime * colorChangeSpeed);

        // Smoothly interpolate alpha separately
        float newAlpha = Mathf.Lerp(currentColor.a, targetAlpha, Time.deltaTime * alphaChangeSpeed);

        // Apply new color with updated alpha
        spriteRenderer.color = new Color(newColor.r, newColor.g, newColor.b, newAlpha);

        // If close enough to target, pick a new random target
        if (Vector3.Distance(new Vector3(newColor.r, newColor.g, newColor.b), new Vector3(targetColor.r, targetColor.g, targetColor.b)) < 0.02f)
        {
            targetColor = palette[Random.Range(0, palette.Length)];
        }
        if (Mathf.Abs(newAlpha - targetAlpha) < 0.02f)
        {
            targetAlpha = Random.Range(0.3f, 1f);
        }
    }
}
*/