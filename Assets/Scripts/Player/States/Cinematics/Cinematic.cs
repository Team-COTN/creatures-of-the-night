using System;
using HSM;
using UnityEngine;

namespace Player.States.Cinematics
{    
    public class CinematicRequest {
        public AnimationClip Clip;
        public Action OnComplete;
        public Vector2? MoveTarget;
        public bool? FaceRight;
    }

    public class Cinematic : State
    {
        readonly PlayerCharacterController player;
        public CinematicRequest ActiveRequest { get; private set; }

        public readonly MovingToTarget MovingToTarget;
        public readonly Animating Animating;

        public Cinematic(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
            MovingToTarget = new MovingToTarget(m, this, player);
            Animating = new Animating(m, this, player);
        }

        public void Enter(CinematicRequest request)
        {
            ActiveRequest = request;
            player.isInCinematic = true;
        }

        protected override State GetDefaultChildState()
        {
            if (ActiveRequest?.MoveTarget.HasValue == true) return MovingToTarget;
            if (ActiveRequest?.Clip != null) return Animating;
            return null;
        }

        protected override (State state, string reason) GetNextState()
        {
            if (!player.isInCinematic)
                return (Machine.GetState<Locomotion.Grounded>(), "Cinematic is over");

            return (null, null);
        }
    }

    public class Animating : State
    {
        readonly PlayerCharacterController player;
        readonly State parent;

        Cinematic Cinematic => Machine.GetState<Cinematic>();

        public Animating(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.parent = parent;
            this.player = player;
        }

        protected override void OnEnter()
        {
            player.SetHorizontalVelocity(0);
            player.SetVerticalVelocity(0);

            var clip = Cinematic.ActiveRequest?.Clip;
            if (clip != null)
                player.PlayerAnimator.PlayClip(clip);
        }

        protected override (State state, string reason) GetNextState()
        {
            if (Cinematic.ActiveRequest?.Clip == null) return (parent, "No clip");
            if (player.PlayerAnimator.IsClipFinished()) return (parent, "Animation complete");
            return (null, null);
        }

        protected override void OnExit()
        {
            player.PlayerAnimator.StopClip();
            Cinematic.ActiveRequest?.OnComplete?.Invoke();
            player.isInCinematic = false;
        }
    }

    public class MovingToTarget : State
    {
        readonly PlayerCharacterController player;
        public readonly Grounded Grounded;
        public readonly Airborne Airborne;

        Cinematic Cinematic => Machine.GetState<Cinematic>();

        public MovingToTarget(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
            Grounded = new Grounded(m, this, player);
            Airborne = new Airborne(m, this, player);
        }

        protected override State GetDefaultChildState() => player.Grounded ? Grounded : Airborne;

        protected override (State state, string reason) GetNextState()
        {
            var target = Cinematic.ActiveRequest?.MoveTarget;
            if (!target.HasValue) return (Machine.GetState<Cinematic>(), "No target");
            if (Vector2.Distance(player.transform.position, target.Value) <= 0.1f)
                return (Machine.GetState<Cinematic>(), "Arrived at target");

            return (null, null);
        }

        protected override void OnExit()
        {
            var request = Cinematic.ActiveRequest;
            if (request == null) return;

            // Snap to final facing direction
            if (request.FaceRight.HasValue && player.isFacingRight != request.FaceRight.Value)
            {
                player.isFacingRight = request.FaceRight.Value;
                player.transform.Rotate(0f, request.FaceRight.Value ? 180f : -180f, 0f);
            }

            // Clear target so Cinematic re-evaluates its default child state
            request.MoveTarget = null;

            // If no clip follows, we're done
            if (request.Clip == null)
            {
                request.OnComplete?.Invoke();
                player.isInCinematic = false;
            }
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            var target = Cinematic.ActiveRequest?.MoveTarget;
            if (!target.HasValue) return;

            bool movingRight = player.transform.position.x < target.Value.x;
            if (player.isFacingRight != movingRight)
            {
                player.isFacingRight = movingRight;
                player.transform.Rotate(0f, movingRight ? 180f : -180f, 0f);
            }

            float direction = movingRight ? 1f : -1f;
            player.SetHorizontalVelocity(direction * player.locomotionData.maxWalkSpeed);
        }
    }

    public class Grounded : State
    {
        readonly PlayerCharacterController player;

        public Grounded(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent) => this.player = player;
        protected override (State state, string reason) GetNextState() => !player.Grounded ? (Machine.GetState<Airborne>(), "Player is not grounded") : (null, null);
        protected override void OnEnter() => player.PlayerAnimator.PlayWalk();
        protected override void OnFixedUpdate(float fixedDeltaTime) => player.SetVerticalVelocity(-0.01f);
    }

    public class Airborne : State
    {
        readonly PlayerCharacterController player;

        public Airborne(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent) => this.player = player;
        protected override (State state, string reason) GetNextState() => player.Grounded ? (Machine.GetState<Grounded>(), "Player is grounded") : (null, null);
        protected override void OnEnter() => player.PlayerAnimator.PlayFall();
        protected override void OnFixedUpdate(float fixedDeltaTime) => player.IncrementVerticalVelocity(player.locomotionData.Gravity * player.locomotionData.gravityFallMultiplier * fixedDeltaTime);
    }
}