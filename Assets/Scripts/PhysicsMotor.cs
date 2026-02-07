using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsMotor : MonoBehaviour
{
    public float CollisionPadding = 0.015f;
    public LayerMask CollisionMask;
    
    [Range(2, 100)] public int HorizontalRayCount = 4;
    [Range(2, 100)] public int VerticalRayCount = 4;
    public RaycastCorners raycastCorners;
    private float _horizontalRaySpacing;
    private float _verticalRaySpacing;
    private BoxCollider2D _col;
    private Rigidbody2D _rb;
    public bool debug;

    public bool IsCollidingAbove { get; private set; }
    public bool IsCollidingBelow { get; private set; }
    public bool IsCollidingRight { get; private set; }
    public bool IsCollidingLeft { get; private set; }

    public struct RaycastCorners
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
    }

    private void Awake()
    {
        _col = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        CalculateRaycastSpacing();
    }

    public void Move(Vector2 velocity)
    {
        UpdateRaycastCorners();
        ResetCollisionStates();

        ResolveHorizontalMovement(ref velocity);
        ResolveVerticalMovement(ref velocity);
        _rb.MovePosition(_rb.position + velocity);
    }
    
    private void ResolveHorizontalMovement(ref Vector2 velocity)
    {
        float rayDir = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + CollisionPadding;
        for (int i = 0; i < HorizontalRayCount; i++)
        {
            Vector2 rayOrigin = (rayDir <= -1) ? raycastCorners.bottomLeft : raycastCorners.bottomRight;
            rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * rayDir, rayLength, CollisionMask);

            if (hit)
            {
                velocity.x = (hit.distance - CollisionPadding) * rayDir;
                rayLength = hit.distance;
                IsCollidingLeft = rayDir <= -1 || IsCollidingLeft;
                IsCollidingRight = rayDir >= 1 || IsCollidingRight;
            }
#if UNITY_EDITOR
            if (debug)
                Debug.DrawRay(rayOrigin, Vector2.right * (rayDir * rayLength), hit ? Color.red : Color.white);
#endif

        }
    }

    private void ResolveVerticalMovement(ref Vector2 velocity)
    {
        float rayDir = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + CollisionPadding;

        for (int i = 0; i < VerticalRayCount; i++)
        {
            Vector2 rayOrigin = (rayDir <= -1) ? raycastCorners.bottomLeft : raycastCorners.topLeft;
            rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * rayDir, rayLength, CollisionMask);

            if (hit)
            {
                velocity.y = (hit.distance - CollisionPadding) * rayDir;
                rayLength = hit.distance;
                IsCollidingBelow = rayDir <= -1 || IsCollidingBelow;
                IsCollidingAbove = rayDir >= 1 || IsCollidingAbove;
            }

#if UNITY_EDITOR
            if (debug)
                Debug.DrawRay(rayOrigin, Vector2.up * (rayDir * rayLength) , hit ? Color.red : Color.cyan);
#endif
        }
    }

    private void ResetCollisionStates()
    {
        IsCollidingAbove = false;
        IsCollidingBelow = false;
        IsCollidingRight = false;
        IsCollidingLeft = false;
    }

    private void UpdateRaycastCorners()
    {
        Bounds bounds = _col.bounds;
        bounds.Expand(CollisionPadding * -2f);
        raycastCorners.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastCorners.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastCorners.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastCorners.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    private void CalculateRaycastSpacing()
    {
        Bounds bounds = _col.bounds;
        bounds.Expand(CollisionPadding * -2f);
        _horizontalRaySpacing = bounds.size.y / (HorizontalRayCount - 1);
        _verticalRaySpacing = bounds.size.x / (VerticalRayCount - 1);
    }

    private void OnDrawGizmos()
    {
        // Gizmos.DrawSphere(raycastCorners.topLeft, 0.1f);
        // Gizmos.DrawSphere(raycastCorners.topRight, 0.1f);
        // Gizmos.DrawSphere(raycastCorners.bottomLeft, 0.1f);
        // Gizmos.DrawSphere(raycastCorners.bottomRight, 0.1f);
    }

    public bool IsGrounded() => IsCollidingBelow;
    public bool BumpedHead() => IsCollidingAbove;
    public bool IsTouchingWall(bool isFacingRight) => (isFacingRight && IsCollidingRight) || (!isFacingRight && IsCollidingLeft);

    public int GetWallDirection()
    {
        if (IsCollidingLeft) return -1;
        if (IsCollidingRight) return 1;
        return 0;
    }
}
