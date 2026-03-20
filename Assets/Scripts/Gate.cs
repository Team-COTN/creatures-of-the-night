using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform pointA; // Starting point
    public Transform pointB; // Target point
    public float speed = .5f; // Units per second
    private Vector3 targetPosition;


    [Header("Detect Cluster")]
    public GameObject[] clusterObjects;

    private bool[] counted;
    private int counter = 0;

    void Start()
    {
        counted = new bool[clusterObjects.Length];
        transform.position = pointA.position;
        targetPosition = pointA.position; //make sure target isn't innitialized to (0, 0, 0)
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0.001f)
        {
            // Move towards the target position
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );
        }

        for (int i = 0; i < clusterObjects.Length; i++)
        {
            if (!counted[i] && clusterObjects[i] == null)
            {
                counted[i] = true;
                counter++;
            }
        }

        Debug.Log("counter: " + counter);

        if (counter == clusterObjects.Length)
        {
            Debug.Log("cluster Complete");
            targetPosition = pointB.position;
        }
    }
}