using UnityEngine;

[CreateAssetMenu(fileName = "New Locomotion Data", menuName = "Create New Locomotion Data")]
public class CharacterLocomotionData : ScriptableObject
{
    [Header("Walk")]
    [Range(0f, 1f)] public float moveThreshold = 0.25f;
    [Range(1f, 100f)] public float maxWalkSpeed = 12.5f;

    [Header("Jumping and Falling")]
    public float jumpHeight = 6.5f;
    public float jumpTime = 0.35f;
    public float gravityOnReleaseMultiplier = 3f;
    public float gravityFallMultiplier = 1.5f;
    public float apexHangTime = 0.075f;
    
    [Header("Dash/Switch-Dash")]
    public float dashDistance = 1.0f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 1f;
    public float switchDashDistance = 3.5f;
    public float switchDashDuration = 0.2f;
    public float switchDashCooldown = 0.15f;
    [Range(0f, 1f)] public float jumpBufferTime = 0.125f;
    [Range(0f, 1f)] public float jumpCoyoteTime = 0.1f;
    
    public float Gravity { get; private set; }
    public float InitialJumpVelocity { get; private set; }

    private void OnValidate() => CalculateValues();
    private void OnEnable() => CalculateValues();
    
    // As described in Math for Game Programmers: Building a Better Jump https://youtu.be/hG9SzQxaCm8?si=kFOlkUusB9AV7hzf
    // Further simplified and modified by Sasquatch B Studios: https://youtu.be/zHSWG05byEc?si=LWoXSxXS3tw-ff3W
    private void CalculateValues()
    {
        Gravity = -(2f * jumpHeight * 1.054f) / Mathf.Pow(jumpTime, 2f); // g = -2h/t^2 and 1.054f is a magic number for height compensation
        InitialJumpVelocity = Mathf.Abs(Gravity) * jumpTime; // v0 = 2h/t simplified as sqrt(g)*t
    }
}
