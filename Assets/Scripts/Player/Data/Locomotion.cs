using UnityEngine;

namespace Player.Data
{
    [CreateAssetMenu(fileName = "New Locomotion Data", menuName = "Create New Locomotion Data")]
    public class Locomotion : ScriptableObject
    {
        [Header("Walk")]
        [Range(0f, 1f)] public float movementInputThreshold = 0.25f;
        public float maxWalkSpeed = 5f;

        [Header("Jump")]
        public float jumpHeight = 6.5f;
        public float jumpTime = 0.35f;
        public float apexHangTime = 0.075f;
        public float jumpCoyoteTime = 0.1f;
        public float jumpBufferTime = 0.125f;

        [Header("Fall")]
        public float gravityOnReleaseMultiplier = 3f;
        public float gravityFallMultiplier = 1.5f;
    
        [Header("Dash")]
        public float dashDistance = 1.0f;
        public float dashDuration = 0.5f;
        public float dashCooldown = 1f;
        public float airDashHangTime = 0.1f;
        
        [Header("Slash")]
        public float slashDuration = .5f;

        [Header("Damage")] 
        public float damagedDuration = .4f;
        public float knockbackDuration = 0.005f;
        
        public float Gravity { get; private set; }
        public float InitialJumpVelocity { get; private set; }

        private void OnValidate() => CalculateValues();
        private void OnEnable() => CalculateValues();
    
        // As described in Math for Game Programmers: Building a Better Jump https://youtu.be/hG9SzQxaCm8?si=kFOlkUusB9AV7hzf
        // Further simplified and modified by Sasquatch B Studios: https://youtu.be/zHSWG05byEc?si=LWoXSxXS3tw-ff3W
        private void CalculateValues()
        {
            Gravity = -(2f * jumpHeight * 1.054f) / Mathf.Pow(jumpTime, 2f); // g = -2h/t^2 with 1.054f as a magic number for height compensation
            InitialJumpVelocity = Mathf.Abs(Gravity) * jumpTime; // v0 = 2h/t simplified as |g|*t
        }
    }
}
