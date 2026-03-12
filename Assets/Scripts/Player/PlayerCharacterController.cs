using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using HSM;
using Player.Data;
using Player.Eye;
using Player.States;
using Player.States.Cinematics;
using Player.States.Locomotion;
using MoreMountains.Feedbacks;
using NaughtyAttributes;

namespace Player
{
    public class PlayerCharacterController : MonoBehaviour, ICharacter
    {
        [Header("References")]
        public PhysicsMotor motor;
        public PlayerAnimator PlayerAnimator;
        public StateMachine Machine;
        private State root;
        public EyeController eye;
        public Vector2 velocity;
        public bool isFacingRight = true;

        public MMF_Player myParryFeedbacks;

        // Remove old cinematic fields - now lives in CinematicRequest
        public bool isInCinematic = false;

        //damaged
        public bool characterBeingDamaged = false;
        public Vector2 _hazardPosition;

        [SerializeField] public Collider2D attackCollider2D;
        [SerializeField] public Collider2D parryCollider2D;

        [Header("Settings")]
        public Locomotion locomotionData;
        
        [Header("Debug")]
        public bool debug = true;
        public float debugInfoPanelHeight = 10f;

        private void Awake()
        {
            root = new Root(null, this);
            var builder = new StateMachineBuilder(root);
            Machine = builder.Build();

            // Machine.OnStateEntered<Idle>(OnIdle);
            // Machine.OnStateEntered<States.Locomotion.Grounded>(OnIdle);
            // Machine.OnStateEntered<Move>(OnMove);
            // Machine.OnStateEntered<Jump>(OnJump);
            // Machine.OnStateEntered<JumpParry>(OnJumpParry);
        }

        // --- Cinematic API ---

        public void EnterCinematic(CinematicRequest request)
        {
            Machine.GetState<Cinematic>().Enter(request);
        }

        public void MoveToPosition(Vector2 position, bool? faceRight = null, Action onComplete = null)
        {
            EnterCinematic(new CinematicRequest {
                MoveTarget = position,
                FaceRight = faceRight ?? (position.x >= transform.position.x),
                OnComplete = onComplete
            });
        }

        [Button] public void ExitCinematic() => isInCinematic = false;

        // --- Animation callbacks ---
        
        // private void OnIdle() => animator.SetTrigger("Idle");

        // private void OnMove()
        // {
        //     if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk Cycle"))
        //         animator.SetTrigger("Move");
        // }

        // private void OnJump() => animator.SetTrigger("Jump");

        // private void OnJumpParry()
        // {
        //     animator.SetTrigger("JumpParry");
        //     myParryFeedbacks.PlayFeedbacks();
        // }

        // private void OnSlash()
        // {
        //     animator.SetTrigger("Slash");
        //     myParryFeedbacks.PlayFeedbacks();
        // }

        // --- Core loop ---

        private void Update()
        {
            Machine.Tick(Time.deltaTime);
            Machine.debug = debug;
        }

        private void FixedUpdate()
        {
            Machine.FixedTick(Time.fixedDeltaTime);
            motor.Move(velocity * Time.fixedDeltaTime);
        }

        // --- Helpers ---

        public void SetVerticalVelocity(float value) => velocity = new Vector2(velocity.x, value);
        public void IncrementVerticalVelocity(float value) => velocity += new Vector2(0, value);
        public void SetHorizontalVelocity(float value) => velocity = new Vector2(value, velocity.y);
        public void IncrementHorizontalVelocity(float value) => velocity += new Vector2(value, 0);
        public bool Grounded => motor.IsGrounded();

        public void SetPosition(Vector2 position, bool? faceRight = null)
        {
            transform.position = position;
            bool shouldFaceRight = faceRight ?? (position.x >= transform.position.x);
            if (shouldFaceRight != isFacingRight)
            {
                isFacingRight = shouldFaceRight;
                transform.Rotate(0f, shouldFaceRight ? 180f : -180f, 0f);
            }
        }

        public void Damage()
        {
            if (Machine.Root.Leaf() != Machine.GetState<Damaged>())
                characterBeingDamaged = true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!debug) return;
            var debugInfoPanelText = "";
            if (Application.isPlaying)
            {
                var statePath = string.Join(" > ", Machine.Root.Leaf().PathToRoot().Reverse().Skip(1).Select(n => n.GetType().Name));
                debugInfoPanelText += statePath;
            }
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.LowerCenter;
            Handles.Label(transform.position + Vector3.up * debugInfoPanelHeight, debugInfoPanelText, centeredStyle);
        }
#endif
    }
}
