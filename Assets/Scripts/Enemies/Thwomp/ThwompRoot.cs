using HSM;
using UnityEngine;

namespace Enemies.Thwomp
{
    public class ThwompRoot : State
    {
        public readonly ThwompEnemy Thwomp;
        public readonly Vector2 HomePosition;

        public readonly Idle Idle;
        public readonly Attacking Attacking;
        public readonly AttackHit AttackHit;
        public readonly AttackMiss AttackMiss;
        public readonly Retracting Retracting;

        public ThwompRoot(StateMachine m, ThwompEnemy thwomp) : base(m, null)
        {
            Thwomp = thwomp;
            HomePosition = thwomp.transform.position;

            Idle = new Idle(m, this);
            Attacking = new Attacking(m, this);
            AttackHit = new AttackHit(m, this);
            AttackMiss = new AttackMiss(m, this);
            Retracting = new Retracting(m, this);
        }

        public override State GetDefaultChildState() => Idle;
    }

    public class Idle : State
    {
        private ThwompRoot Root => (ThwompRoot)Parent;
        private ThwompEnemy Thwomp => Root.Thwomp;

        public Idle(StateMachine m, State parent) : base(m, parent) { }

        protected override (State state, string reason) GetNextState()
        {
            if (Thwomp.IsPlayerBelow())
                return (Root.Attacking, "player detected below");

            return (null, null);
        }
    }

    public class Attacking : State
    {
        private ThwompRoot Root => (ThwompRoot)Parent;
        private ThwompEnemy Thwomp => Root.Thwomp;

        public Attacking(StateMachine m, State parent) : base(m, parent) { }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            Thwomp.Motor.Move(Vector2.down * (Thwomp.ChargeSpeed * fixedDeltaTime));
        }

        protected override (State state, string reason) GetNextState()
        {
            if (Thwomp.IsPlayerInHitBox())
                return (Root.AttackHit, "hit player");

            if (Thwomp.Motor.IsGrounded())
                return (Root.AttackMiss, "reached ground, no player in range");

            return (null, null);
        }
    }

    public class AttackHit : State
    {
        private ThwompRoot Root => (ThwompRoot)Parent;
        private ThwompEnemy Thwomp => Root.Thwomp;
        private float _timer;

        public AttackHit(StateMachine m, State parent) : base(m, parent) { }

        protected override void OnEnter()
        {
            _timer = 0f;
            // Hook your damage/knockback/VFX/SFX up to this event in the Inspector,
            // or subscribe from code: thwomp.OnPlayerHit.AddListener(...)
            Thwomp.OnPlayerHit?.Invoke(Thwomp.gameObject);
        }

        protected override void OnUpdate(float deltaTime)
        {
            _timer += deltaTime;
        }

        protected override (State state, string reason) GetNextState()
        {
            if (_timer >= Thwomp.HitPauseDuration)
                return (Root.Retracting, "hit pause elapsed");

            return (null, null);
        }
    }

    public class AttackMiss : State
    {
        private ThwompRoot Root => (ThwompRoot)Parent;
        private ThwompEnemy Thwomp => Root.Thwomp;
        private float _timer;

        public AttackMiss(StateMachine m, State parent) : base(m, parent) { }

        protected override void OnEnter()
        {
            _timer = 0f;
        }

        protected override void OnUpdate(float deltaTime)
        {
            _timer += deltaTime;
        }

        protected override (State state, string reason) GetNextState()
        {
            if (_timer >= Thwomp.MissPauseDuration)
                return (Root.Retracting, "miss pause elapsed");

            return (null, null);
        }
    }

    public class Retracting : State
    {
        private ThwompRoot Root => (ThwompRoot)Parent;
        private ThwompEnemy Thwomp => Root.Thwomp;

        public Retracting(StateMachine m, State parent) : base(m, parent) { }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            float remaining = Root.HomePosition.y - Thwomp.transform.position.y;
            if (remaining <= 0f) return;

            float step = Mathf.Min(Thwomp.RetractSpeed * fixedDeltaTime, remaining);
            Thwomp.Motor.Move(Vector2.up * step);
        }

        protected override (State state, string reason) GetNextState()
        {
            const float epsilon = 0.01f;
            if (Thwomp.transform.position.y >= Root.HomePosition.y - epsilon)
                return (Root.Idle, "reached home position");

            return (null, null);
        }
    }
}