using HSM;
using System.Linq;
using Player.States.Locomotion;
using Player.States.Cinematics;
using UnityEngine;

namespace Player.States
{
    public class Root : State
    {
        readonly PlayerCharacterController player;
        public readonly Locomotion.Grounded Grounded;
        public readonly Locomotion.Airborne Airborne;
        public readonly Scrying Scrying;
        public readonly Damaged Damaged;
        public readonly Cinematic Cinematic;


        public float JumpBufferTimer;
        public float CoyoteTimer;
        public float invincibleTimer;
        
        public Root(StateMachine m, PlayerCharacterController player) : base(m, null)
        {
            this.player = player;
            Grounded = new Locomotion.Grounded(m, this, player);
            Airborne = new Locomotion.Airborne(m, this, player);
            Scrying = new Scrying(m, this, player);
            Damaged = new Damaged(m, this, player);
            Cinematic = new Cinematic(m, this, player);
        }

        public void setInvinsibleTimer(float time)
        {
            invincibleTimer = time;
        }

        public override State GetDefaultChildState() => Grounded;
        
        protected override void OnUpdate(float deltaTime)
        {
            // If the player has been damaged and is now invincible, count down the invincible timer
            if (player.characterIsinvincibile)
                invincibleTimer -= deltaTime;
            // if invincibility runs out, no longer invincible (can be damaged)
            if (invincibleTimer <= 0)
                player.characterIsinvincibile = false;
        }
        protected override (State state, string reason) GetNextState()
        {
            if (Leaf() == Damaged) return (null, null);

            //if CharacterInteractions invokes PlayerTakeDamage *** depricated. Now PlayerCharController does this!
            // if (player.characterBeingDamaged && !player.characterIsinvincibile)
            if (player.characterBeingDamaged && !player.characterIsinvincibile)
                return (Machine.GetState<Damaged>(), "Player got damaged!");

            if (player.isInCinematic && !Leaf().PathToRoot().Contains(Machine.GetState<Cinematic>()))
                return (Machine.GetState<Cinematic>(), "Cinematic started");

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
            player.characterIsinvincibile = true;
            Machine.GetState<Root>().setInvinsibleTimer(player.locomotionData.invincibleDuration);
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
                Vector3 hazardPos3D = new Vector3(player._hazardPosition.x, player._hazardPosition.y, 0);
                float magnitude = Mathf.Lerp(player.locomotionData.knockbackForce, 1f, knockbackDuration);
                Vector3 direction = (player.transform.position - hazardPos3D).normalized;
                Vector3 velocity = direction * magnitude;

                player.SetHorizontalVelocity(velocity.x);
                player.SetVerticalVelocity(velocity.y);
            } else
            {
                player.IncrementVerticalVelocity(player.locomotionData.Gravity * player.locomotionData.gravityFallMultiplier * fixedDeltaTime);            
            }
        }

        protected override void OnExit()
        {
            player.characterBeingDamaged = false;
        }
    }
}
