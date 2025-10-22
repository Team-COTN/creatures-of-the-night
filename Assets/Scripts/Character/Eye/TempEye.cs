using System;
using UnityEngine;
using UnityEngine.Rendering.Universal; //for 2D light
// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using Unity.VisualScripting;
// using UnityEngine;

//reimplementing eye from old demo
//will need to be updated once heirarchical state machine is completed
public class TempEye : MonoBehaviour
{
    private Transform playerTransform;
    bool eyeWasPressedThisFrame => InputManager.GetEyeWasPressedThisFrame();
    bool enableEyeControls = false;
    private float Xoffset = 3f;
    private float Yoffset = 1f;
    public float circleRangeMin = 80;
    public float circleRangeMax = 100;
    private float eyeSpeed = 20;
    string eyeState;
    string[] eyeStates = new string[] {"idle", "scry"};
    public float smoothness = 50f;

    [SerializeField] Light2D eyePointLight;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        eyeState = eyeStates[0];
        spriteRenderer = GetComponent<SpriteRenderer>();
        eyePointLight.pointLightOuterRadius = 3;

    }
    private void Update()
    {
        if (eyeWasPressedThisFrame)
        {
            enableEyeControls = !enableEyeControls;
        }


        if (enableEyeControls)
        {
            eyePointLight.pointLightOuterRadius = 7;

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
        }
        if (eyeState == eyeStates[0])
        {
            eyePointLight.pointLightOuterRadius = 3;

            // Disable the sprite renderer
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;

            if (playerTransform != null)
            {
                Vector2 offset = new Vector2(Xoffset, Yoffset);

                // Set the position of this GameObject to the reference GameObject's position plus the offset
                Vector2 targetPosition = (Vector2)playerTransform.position + offset;
                transform.position = Vector2.Lerp(transform.position, targetPosition, smoothness * Time.deltaTime);
            }

        }
    }
    
}
