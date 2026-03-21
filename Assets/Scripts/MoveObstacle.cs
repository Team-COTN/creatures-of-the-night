using UnityEngine;

public class MoveObstacle : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform pointA; // Starting point
    public Transform pointB; // Target point
    public float speed = .5f; // Units per second

    private Vector3 targetPosition;

    void Start()
    {
        // // Validate references
        // if (pointA == null || pointB == null)
        // {
        //     Debug.LogError("Point A and Point B must be assigned in the Inspector.");
        //     enabled = false;
        //     return;
        // }

        // Start at Point A
        transform.position = pointA.position;
        targetPosition = pointB.position;
    }

    void Update()
    {
        // Move towards the target position
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // If reached target, swap target to create back-and-forth motion
        if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
        {
            targetPosition = targetPosition == pointA.position ? pointB.position : pointA.position;
        }
    }

}
