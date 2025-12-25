using System;
using UnityEngine;
using UnityEngine.Rendering.Universal; //for 2D light

//reimplementing eye from old demo
//will need to be updated once heirarchical state machine is completed
public class TempEye : MonoBehaviour
{
    private Character character;
    private Animator eyeAnimator;
    private Transform playerTransform;
    public Collider2D illuminetRicochetCollider;
    private bool eyeWasPressedThisFrame => InputManager.GetEyeWasPressedThisFrame();
    private bool IlluminetRicochetWasPressedThisFrame => InputManager.GetIlluminetRicochetWasPressedThisFrame();
    private bool enableEyeControls = false;
    private bool enableIlluminetRicochet = false;
    private float Xoffset = 2.03f;
    private float Yoffset = 0.98f;
    private float XoffsetLeft = 1.26f;

    private float circleRangeMin = 8f;
    private float circleRangeMax = 10f;
    private float eyeSpeed = 10f;
    private float smoothness = 50f;
    private float minLightRadius = 2.5f;
    private float maxLightRadius = 7f;
    private float transitionSpeed = 1f;

    private bool check = true;
    
    [SerializeField] Light2D eyePointLight;

    private SpriteRenderer spriteRenderer;
    
    public event Action<Transform> CharacterEyeStateChange;
    public void AddEyeStateChangeObserver(Action<Transform> observer) { CharacterEyeStateChange += observer; }
    public void RemoveEyeStateChangeObserver(Action<Transform> observer) { CharacterEyeStateChange -= observer; }

    void Start()
    {
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        playerTransform = character.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        eyePointLight.pointLightOuterRadius = minLightRadius;
        eyeAnimator = this.gameObject.GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        if (eyeWasPressedThisFrame)
        {
            enableEyeControls = !enableEyeControls;
        }
        
        if (enableEyeControls)
        {
            CharacterEyeStateChange?.Invoke(transform);
            eyePointLight.pointLightOuterRadius = Mathf.Lerp(eyePointLight.pointLightOuterRadius, maxLightRadius, Time.deltaTime * transitionSpeed);
            
            // Enable the sprite renderer
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;
        
            // Obtain raw player input
            Vector2 playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            // Calculate the difference vector from the player to the eye
            Vector2 delta = playerTransform.position - transform.position;

            // Determine the magnitude and normalized direction of the difference
            float distanceToCenter = delta.magnitude;
            Vector2 directionToCenterNormalized = delta.normalized;

            // Set speed multiplier based on distance from the player to the eye
            float speedMultiplierByDistance = 1f - Mathf.InverseLerp(circleRangeMin, circleRangeMax, distanceToCenter);

            // Determine speed multiplier based on the direction of movement towards or away from the center
            float speedMultiplierByDirection = Mathf.Clamp(Vector2.Dot(playerInput, directionToCenterNormalized), 0, 1);

            // Choose the larger multiplier between distance and direction-based multipliers
            float speedMultiplier = Mathf.Max(speedMultiplierByDistance, speedMultiplierByDirection);

            // Move the eye using player input, eye speed, and the determined speed multiplier
            transform.Translate(playerInput * eyeSpeed * speedMultiplier * Time.deltaTime);
            
            //IlluminetRicochet logic
            if (IlluminetRicochetWasPressedThisFrame)
                enableIlluminetRicochet = !enableIlluminetRicochet;

            if (enableIlluminetRicochet)
            {
                illuminetRicochetCollider.enabled = true;

                //animator
                if (check)
                {
                    eyeAnimator.SetTrigger("Illuminate");
                    check = false;
                }
            }

            if (!enableIlluminetRicochet)
            {
                illuminetRicochetCollider.enabled = false;
                check = true;
            }
        }
        if (!enableEyeControls)
        {
            CharacterEyeStateChange?.Invoke(playerTransform);
            eyePointLight.pointLightOuterRadius = Mathf.Lerp(eyePointLight.pointLightOuterRadius, minLightRadius, Time.deltaTime * transitionSpeed);
            
            //don't let eye states persist
            enableIlluminetRicochet = false;
            illuminetRicochetCollider.enabled = false;
            check = true;
            
            // Disable the sprite renderer
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;

            if (playerTransform != null)
            {
                //by default assume the player is facing right
                Vector2 offset = new Vector2(Xoffset, Yoffset);
                if (!character.IsFacingRight)
                {
                    offset = new Vector2(XoffsetLeft, Yoffset);
                }
                // Set the position of this GameObject to the reference GameObject's position plus the offset
                Vector2 targetPosition = (Vector2)playerTransform.position + offset;
                transform.position = Vector2.Lerp(transform.position, targetPosition, smoothness * Time.deltaTime);
            }

        }
    }
    
}
