
using HSM;
using UnityEngine;

namespace Enemies.Destractable
{

[RequireComponent(typeof(PhysicsMotor))]
public class DistractableEnemy : StateMachineMonoBehaviour, IDamagable, IShootable, IKnockable, IBlockable
{
    [Header("References")]
    public Transform player;

    [Header("Detection")]
    public float attackDetectionRange = 1f;
    public float detectionRange = 5f;
    public float chaseDetectionRange = 7f;
    public float wanderSightHeight = 1f;
    

    [Header("Movement")]
    public float chaseSpeed = 2.5f;
    public float wanderSpeed = 1f;
    public float gravity = 10f;

    [Header("Damage")]
    public bool canBeDamaged = false;
    public bool enemyIsinvincibile = false;
    public int enemyHP = 5;
    public int damageTaken = 0;
    public Vector2 _hazardPosition;
    public float damagedDuration = 2f;
    public float knockbackDuration = 0.15f;
    public float knockbackForce = .3f;

    [Header("Attack")]
    public Collider2D attackCollider2D;
    public float attackRadius = 2f;
    public float attackDuration = 0.5f;

    public float attackCooldown = 1.5f;
    public float attackCooldownTimer = 0f;


    [Header("Timing")]
    public float wanderDirectionChangeInterval = 2f;
    public float rechaseDelay = 2f;
    public float ledgeGiveUpDelay = 2f;

    [Header("Ledge Detection")]
    public float ledgeCheckAhead = 0.3f;
    public float groundProbeDepth = 0.2f;

    private PhysicsMotor _motor;
    public PhysicsMotor Motor => _motor ??= GetComponent<PhysicsMotor>();
    private Collider2D _col;
    public Collider2D Col => _col ??= GetComponent<Collider2D>();

    public Vector2 velocity;
    public void SetHorizontalVelocity(float value) => velocity = new Vector2(value, velocity.y);
    public void SetVerticalVelocity(float value) => velocity = new Vector2(velocity.x, value);

    protected override State CreateRootState() => new Root(null, this);

    public bool IsPlayerWithinRange(float range) =>
        player != null && Vector2.Distance(transform.position, player.position) <= range;

    public bool CanSeePlayerHorizontally(float range)
    {
        if (player == null) return false;
        return Mathf.Abs(player.position.x - transform.position.x) <= range
            && Mathf.Abs(player.position.y - transform.position.y) <= wanderSightHeight;
    }

    public bool IsGroundAheadOf(float direction) => IsGroundAheadOf(direction, out _, out _);

    public bool IsGroundAheadOf(float direction, out Vector2 origin, out Vector2 checkPoint)
    {
        Vector2 probeOrigin = new Vector2(transform.position.x, Col.bounds.min.y);
        RaycastHit2D groundHit = Physics2D.Raycast(probeOrigin, Vector2.down, groundProbeDepth, Motor.CollisionMask);

        if (groundHit.collider == null)
        {
            origin = checkPoint = probeOrigin;
            return false;
        }

        origin = groundHit.point;
        float lookAhead = Mathf.Max(ledgeCheckAhead, chaseSpeed * Time.fixedDeltaTime);
        checkPoint = new Vector2(groundHit.point.x + direction * lookAhead, groundHit.point.y - 0.05f);

        return Physics2D.OverlapPoint(checkPoint, Motor.CollisionMask) != null;
    }

    // public void Die() => Debug.Log("die");
    public void Die() => UnityEngine.Object.Destroy(this.gameObject);

    public void TakeShotDamage(int damageAmount)
    {
        //transition to damaged state
        canBeDamaged = true;
        Debug.Log("canBeDamaged = true");

        //
        if (enemyIsinvincibile) 
            return;
        damageTaken += damageAmount; 
    }
    
    public void TakeDamage(int damageAmount)
    {
        Debug.Log("I am taking damage");
        if (enemyIsinvincibile)
            return;
        Debug.Log("enemy not invincible");

        enemyIsinvincibile = true;
        //transition to damaged state
        canBeDamaged = true;
        //take damage
        damageTaken += damageAmount;
    }
    

    public void TakeKnockback(Vector2 hazardPosition)
    {
        _hazardPosition = hazardPosition;
    }

    public void GetBlocked()
    {
        Debug.Log("My attack has been blocked!");
    }
}

public class Root : State
{
    public readonly DistractableEnemy Enemy;
    public readonly Wander Wander;
    public readonly Chase Chase;
    public readonly Damaged Damaged;
    public readonly Attack Attack;

    public Root(StateMachine m, DistractableEnemy enemy) : base(m, null)
    {
        Enemy = enemy;
        Wander = new Wander(m, this);
        Chase = new Chase(m, this);
        Damaged = new Damaged(m, this);
        Attack = new Attack(m, this);
    }

    public override State GetDefaultChildState() => Wander;

    protected override void OnFixedUpdate(float fixedDeltaTime)
    {
        Enemy.SetVerticalVelocity(Enemy.Motor.IsGrounded() ? 0f : Enemy.velocity.y - Enemy.gravity * fixedDeltaTime);
        Enemy.Motor.Move(Enemy.velocity * fixedDeltaTime);
        
        Enemy.attackCooldownTimer -= fixedDeltaTime;

        if (Enemy.enemyHP <= Enemy.damageTaken)
            Enemy.Die();
    }

    public override void DrawCustomGizmos()
    {
        Gizmos.color = Enemy.Motor.IsGrounded() ? Color.green : Color.red;
        Vector3 feet = Enemy.transform.position + Vector3.down * Enemy.Col.bounds.extents.y;
        Gizmos.DrawWireCube(feet, new Vector3(Enemy.Col.bounds.size.x, 0.05f, 0f));
    }

    protected override (State state, string reason) GetNextState()
    {
        if (Enemy.canBeDamaged)
        {
            Enemy.canBeDamaged = false;
            return (Damaged, "enemy was damaged");
        }
        //could be in chase? Since it will never be *very close* if it is not at least ~kinda close~
        if (Enemy.IsPlayerWithinRange(Enemy.attackDetectionRange) && Enemy.attackCooldownTimer <= 0f)
            return (Attack, "can attack player");

        //outside root it would be:       return (Root.Attack, "can attack player");

        return (null, null);
    }
}

public class Wander : State
{
    private Root Root => (Root)Parent;
    private DistractableEnemy Enemy => Root.Enemy;

    private float _direction;
    private float _directionTimer;
    private float _rechaseCooldown;

    public Wander(StateMachine m, State parent) : base(m, parent) { }

    protected override void OnEnter()
    {
        PickNewDirection();
        _rechaseCooldown = Enemy.rechaseDelay;
    }

    protected override void OnExit() => Enemy.SetHorizontalVelocity(0f);

    protected override void OnFixedUpdate(float fixedDeltaTime)
    {
        _rechaseCooldown = Mathf.Max(0f, _rechaseCooldown - fixedDeltaTime);

        _directionTimer -= fixedDeltaTime;
        if (_directionTimer <= 0f) PickNewDirection();

        bool hitWall = (_direction > 0f && Enemy.Motor.IsCollidingRight) ||
                       (_direction < 0f && Enemy.Motor.IsCollidingLeft);

        if (hitWall || !Enemy.IsGroundAheadOf(_direction))
        {
            PickNewDirection();
            return;
        }

        Enemy.SetHorizontalVelocity(_direction * Enemy.wanderSpeed);
    }

    protected override (State state, string reason) GetNextState()
    {
        if (_rechaseCooldown <= 0f && Enemy.CanSeePlayerHorizontally(Enemy.detectionRange))
            return (Root.Chase, "player detected");

        return (null, null);
    }

    private void PickNewDirection()
    {
        _direction = Random.value < 0.5f ? -1f : 1f;
        _directionTimer = Enemy.wanderDirectionChangeInterval;
    }

    public override void DrawCustomGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Enemy.transform.position,
            new Vector3(Enemy.detectionRange * 2f, Enemy.wanderSightHeight * 2f, 0f));

        bool groundAhead = Enemy.IsGroundAheadOf(_direction, out Vector2 origin, out Vector2 checkPoint);
        Gizmos.color = groundAhead ? Color.cyan : Color.red;
        Gizmos.DrawLine(origin, checkPoint);
        Gizmos.DrawWireSphere(checkPoint, 0.05f);
    }
}

public class Chase : State
{
    private Root Root => (Root)Parent;
    private DistractableEnemy Enemy => Root.Enemy;

    private float _blockedTimer;

    public Chase(StateMachine m, State parent) : base(m, parent) { }

    protected override void OnEnter() => _blockedTimer = 0f;
    protected override void OnExit() => Enemy.SetHorizontalVelocity(0f);

    protected override void OnFixedUpdate(float fixedDeltaTime)
    {
        float moveDir = MoveDir();
        bool groundAhead = Enemy.IsGroundAheadOf(moveDir);

        Enemy.SetHorizontalVelocity(groundAhead ? moveDir * Enemy.chaseSpeed : 0f);
        _blockedTimer = groundAhead ? 0f : _blockedTimer + fixedDeltaTime;
    }

    protected override (State state, string reason) GetNextState()
    {
        if (!Enemy.IsPlayerWithinRange(Enemy.chaseDetectionRange))
            return (Root.Wander, "lost player");

        if (_blockedTimer >= Enemy.ledgeGiveUpDelay)
            return (Root.Wander, "couldn't reach player - gave up at the ledge");

        return (null, null);
    }

    private float MoveDir() => Enemy.player.position.x >= Enemy.transform.position.x ? 1f : -1f;

    public override void DrawCustomGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f); // orange - visually distinct from Wander's yellow
        Gizmos.DrawWireSphere(Enemy.transform.position, Enemy.chaseDetectionRange);

        if (Enemy.player == null) return;

        float moveDir = MoveDir();
        bool groundAhead = Enemy.IsGroundAheadOf(moveDir, out Vector2 origin, out Vector2 checkPoint);

        Gizmos.color = groundAhead ? Color.green : Color.red;
        Gizmos.DrawLine(origin, checkPoint);
        Gizmos.DrawWireSphere(checkPoint, 0.05f);
    }
}


public class Damaged : State
{
    private Root Root => (Root)Parent;
    private DistractableEnemy Enemy => Root.Enemy;

    private float damagedTimer;
    private float knockbacktimer;


    public Damaged(StateMachine m, State parent) : base(m, parent) { }

    protected override void OnEnter()
    {
        Debug.Log("entered damaged state");
        Enemy.enemyIsinvincibile = true;
        damagedTimer = 0f;
        knockbacktimer = 0f;
    }
    
    protected override void OnFixedUpdate(float fixedDeltaTime)
    {   
        damagedTimer += fixedDeltaTime;
        knockbacktimer += fixedDeltaTime;

        if (knockbacktimer <= Enemy.knockbackDuration)
        {
            Vector3 hazardPos3D = new Vector3(Enemy._hazardPosition.x, Enemy._hazardPosition.y, 0);
            float magnitude = Mathf.Lerp(Enemy.knockbackForce, 1f, Enemy.knockbackDuration);
            Vector3 direction = (Enemy.transform.position - hazardPos3D).normalized;
            Vector3 velocity = direction * magnitude;
            Enemy.SetHorizontalVelocity(velocity.x);
            Enemy.SetVerticalVelocity(velocity.y);
        }
    }

    protected override void OnExit() => Enemy.enemyIsinvincibile = false;
    
    protected override (State state, string reason) GetNextState()
    {
        if (damagedTimer >= Enemy.damagedDuration)
            return (Root.Wander, "done getting damaged");

        return (null, null);
    }
}

public class Attack : State
{
    private Root Root => (Root)Parent;
    private DistractableEnemy Enemy => Root.Enemy;

    private float attackTimer;

    public Attack(StateMachine m, State parent) : base(m, parent) { }

    protected override void OnEnter()
    {
        Debug.Log("entered attack state");
        Enemy.attackCooldownTimer = Enemy.attackCooldown;
        attackTimer = 0f;
    }
    
    protected override void OnFixedUpdate(float fixedDeltaTime)
    {   
        Vector2 weaponColOrigin = Enemy.attackCollider2D.bounds.center;
        Collider2D[] otherCol = Physics2D.OverlapCircleAll(weaponColOrigin, Enemy.attackRadius, ~0);
        for (int i = 0; i < otherCol.Length; i++)
        {
            if (otherCol[i].gameObject.TryGetComponent(out IDamagable damagable))
            {
                if (otherCol[i].gameObject != Enemy.gameObject)
                    damagable.TakeDamage(1);
            }
        }
        attackTimer += fixedDeltaTime;
    }

   
    protected override (State state, string reason) GetNextState()
    {
        if (attackTimer >= Enemy.attackDuration)
            return (Root.Chase, "done trying to attack");

        return (null, null);
    }
}
}