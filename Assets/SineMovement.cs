using UnityEngine;

public class SineMovement : MonoBehaviour
{
    // The distance the object will move up and down from its starting point
    public float movementDistance = 5f; 
    
    // The speed of the movement (higher value means faster oscillation)
    public float speed = 2f; 
    public float offset = 3.14159f/2f; 


    private Vector3 startPosition;

    void Start()
    {
        // Store the initial position of the GameObject
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new Y position using a sine wave
        // Time.time gives a continuously increasing value
        // Mathf.Sin(Time.time * speed) oscillates between -1 and 1
        // We multiply by movementDistance / 2 to get the desired range (e.g., -2.5 to 2.5)
        float newY = startPosition.y + Mathf.Sin(Time.time * speed + offset) * (movementDistance / 2f);

        // Update the object's position
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
