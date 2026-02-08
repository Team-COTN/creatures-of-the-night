using HSM;
using UnityEngine;

namespace Player.States.Locomotion
{
    public class Scrying : State
    {
        readonly PlayerCharacterController player;

        public Scrying(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
        }

        protected override (State state, string reason) GetNextState()
        {
            // Stop controlling the eye on button press
            if (InputManager.GetEyeWasPressedThisFrame())
                return (Machine.GetState<Grounded>(), "Player pressed eye button");
            
            return (null, null);
        }
        
        protected override void OnEnter()
        {
            player.eye.ActivateEye();
        }

        protected override void OnExit()
        {
            player.eye.DeactivateEye();
        }

        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            // Keep the player still
            player.SetHorizontalVelocity(0f);
            if (player.Grounded)
                player.SetVerticalVelocity(-0.01f);
        }
    }
}