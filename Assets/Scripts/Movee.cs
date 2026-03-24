using UnityEngine;

public class Movee : MonoBehaviour
{
    public float moveSpeed = -.5f;
    // Vector2 initialPosition = new Vector2(23, 15);
    
    void Start()
    {
        // Save the initial position when the scene starts
        // initialPosition = transform.position;

        // Start the coroutine that handles the repeated resetting
        // StartCoroutine(ResetObjectPositionRoutine());
    }
    
    void Update()
    {
        // Source - https://stackoverflow.com/a
        // Posted by I.B, modified by community. See post 'Timeline' for change history
        // Retrieved 2025-12-17, License - CC BY-SA 3.0
        this.transform.position += transform.up * moveSpeed * Time.deltaTime;

    }
    // Coroutine to reset the object's position every 3 seconds
    // IEnumerator ResetObjectPositionRoutine()
    // {
    //     while (true) // Infinite loop to keep the routine running
    //     {
    //         // Wait for 3 seconds before the next action
    //         yield return new WaitForSeconds(3f);
    //
    //         // Reset the object's position to the stored initial position
    //         transform.position = initialPosition;
    //     }
    // }
    
    // Update is called once per frame

}
