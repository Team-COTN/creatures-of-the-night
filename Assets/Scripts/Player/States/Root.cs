using HSM;
using Player.States.Locomotion;
using UnityEngine;

namespace Player.States
{
    public class Root : State
    {
        readonly PlayerCharacterController player;
        public readonly Grounded Grounded;
        public readonly Airborne Airborne;
        public readonly Damaged Damaged;


        public float JumpBufferTimer;
        public float CoyoteTimer;
        
        public Root(StateMachine m, PlayerCharacterController player) : base(m, null)
        {
            this.player = player;
            Grounded = new Grounded(m, this, player);
            Airborne = new Airborne(m, this, player);
            Damaged = new Damaged(m, this, player);
        }

        protected override State GetDefaultChildState() => Grounded;
        protected override (State state, string reason) GetNextState()
        {
            //if CharacterInteractions invokes PlayerTakeDamage
            if (player.characterBeingDamaged)
                return (Machine.GetState<Damaged>(), "Player got damaged!");

            return (null, null);
        }
    }

    public class Damaged : State
    {
        readonly PlayerCharacterController player;
        private float damagedDuration;
        private float knockbackDuration;

        
        public Damaged(StateMachine m, State parent, PlayerCharacterController player) : base(m, parent)
        {
            this.player = player;
        }

        protected override (State state, string reason) GetNextState()
        {
            if (damagedDuration >= player.locomotionData.damagedDuration)
            { 
                return (Machine.GetState<Idle>(), "Player finished being affected by damage");
            }
    
            return (null, null);
        }
        
        protected override void OnEnter()
        {
            damagedDuration = 0f;
            knockbackDuration = 0f;
        }


        protected override void OnUpdate(float deltaTime)
        {
            damagedDuration += deltaTime;
            knockbackDuration += deltaTime;
        }
        
        protected override void OnFixedUpdate(float fixedDeltaTime)
        {
            if (knockbackDuration <= player.locomotionData.knockbackDuration)
            {
                Debug.Log(knockbackDuration + " <= " + player.locomotionData.knockbackDuration);
                float knockbackForce = 8f;

                Vector3 hazardPos3D = new Vector3(player._hazardPosition.x, player._hazardPosition.y, 0);
                float magnitude = Mathf.Lerp(knockbackForce, 0f, knockbackDuration);
                Vector3 direction = (player.transform.position - hazardPos3D).normalized;
                Vector3 velocity = direction * magnitude;

                player.SetHorizontalVelocity(velocity.x);
                player.SetVerticalVelocity(velocity.y);

                // Vector3 magnitude = Vector3.Lerp(knockbackForce, knockbackForce/2, knockbackDuration);
                // Vector3 velocity = new Vector3(magnitude * player.transform.position.x, magnitude * player.transform.position.y, magnitude * player.transform.position.z);
                

                
                // float knockbackForce = 5f;
                // Vector3 hazardPos3D = new Vector3(player._hazardPosition.x, player._hazardPosition.y, 0);
                // Vector3 direction = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z) - hazardPos3D;
                // direction = direction.normalized;
                // float magnitude = Mathf.Lerp(knockbackForce, knockbackForce/2, knockbackDuration);
                // Vector3 velocity = new Vector3(magnitude * direction.x, magnitude * direction.y, magnitude * direction.z);
                // player.SetHorizontalVelocity(velocity);
                // player.SetVerticalVelocity(velocity);




                // player.SetVerticalVelocity(direction.y * knockbackForce);
                // player.SetHorizontalVelocity(direction.x * knockbackForce);


                // var finalVelocity = player.locomotionData.maxWalkSpeed;
                // var initialVelocity = (2 * distance / duration) - finalVelocity;
                // var t = Mathf.Clamp01(dashTimer / player.locomotionData.dashDuration);
                // var velocity = Mathf.Lerp(initialVelocity, finalVelocity, t);
                // var direction = player.isFacingRight ? 1f : -1f;
                // player.SetHorizontalVelocity(velocity * direction);
            
            }
        }

        protected override void OnExit()
        {
            player.characterBeingDamaged = false;
        }
    }
}
