using HSM;

namespace Enemies.Thwomp
{
    public class ThwompRoot : State
    {
        private readonly ThwompEnemy thwomp;
        public readonly Idle Idle;
        public readonly Attacking Attacking;
        public readonly AttackHit AttackHit;
        public readonly AttackMiss AttackMiss;
        public readonly Retracting Retracting;
    
        public ThwompRoot(StateMachine m, ThwompEnemy thwomp) : base(m, null)
        {
            this.thwomp = thwomp;
            Idle = new Idle(m, this, thwomp);
            Attacking = new Attacking(m, this, thwomp);
            AttackHit = new AttackHit(m, this, thwomp);
            AttackMiss = new AttackMiss(m, this, thwomp);
            Retracting = new Retracting(m, this, thwomp);
        }

        public override State GetDefaultChildState() => Idle;
    }
    
    public class Idle : State
    {
        private readonly ThwompEnemy thwomp;
        public Idle(StateMachine m, State parent, ThwompEnemy thwompEnemy) : base(m, parent)
        {
            this.thwomp = thwompEnemy;
        }
    }

    public class Attacking : State
    {
        private readonly ThwompEnemy thwomp;
        public Attacking(StateMachine m, State parent, ThwompEnemy thwompEnemy) : base(m, parent)
        {
            this.thwomp = thwompEnemy;
        }
    }

    public class AttackHit : State
    {
        private readonly ThwompEnemy thwomp;
        public AttackHit(StateMachine m, State parent, ThwompEnemy thwompEnemy) : base(m, parent)
        {
            this.thwomp = thwompEnemy;
        }
    }

    public class AttackMiss : State
    {
        private readonly ThwompEnemy thwomp;
        public AttackMiss(StateMachine m, State parent, ThwompEnemy thwompEnemy) : base(m, parent)
        {
            this.thwomp = thwompEnemy;
        }
    }

    public class Retracting : State
    {
        private readonly ThwompEnemy thwomp;
        public Retracting(StateMachine m, State parent, ThwompEnemy thwompEnemy) : base(m, parent)
        {
            this.thwomp = thwompEnemy;
        }
    }
}