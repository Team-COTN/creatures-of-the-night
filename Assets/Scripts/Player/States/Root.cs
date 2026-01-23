using HSM;
using Player.States.Locomotion;

namespace Player.States
{
    public class Root : State
    {
        readonly PlayerCharacterController player;
        public readonly Grounded Grounded;
        public readonly Airborne Airborne;

        public float JumpBufferTimer;
        public float CoyoteTimer;

        public Root(StateMachine m, PlayerCharacterController player) : base(m, null)
        {
            this.player = player;
            Grounded = new Grounded(m, this, player);
            Airborne = new Airborne(m, this, player);
        }

        protected override State GetDefaultChildState() => Grounded;
        protected override State GetNextState() => null;
    }
}
