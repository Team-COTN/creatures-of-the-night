using HSM;
using Player.States.Locomotion;

namespace Player.States
{
    public class Root : State
    {
        readonly PlayerCharacterController player;
        public readonly Grounded Grounded;
        public readonly Airborne Airborne;
        public readonly Scrying Scrying;

        public float JumpBufferTimer;
        public float CoyoteTimer;
        
        public Root(StateMachine m, PlayerCharacterController player) : base(m, null)
        {
            this.player = player;
            Grounded = new Grounded(m, this, player);
            Airborne = new Airborne(m, this, player);
            Scrying = new Scrying(m, this, player);
        }

        protected override State GetDefaultChildState() => Grounded;
    }
}
