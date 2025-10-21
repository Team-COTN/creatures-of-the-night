using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginningCamera : MonoBehaviour
{
    /*
    public GameObject PlayerGameObject; //ref to Player

    public float yOffset = .85f;
    public float zoomSpeed = .15f; //Adjust this value to control zoom speed
    public float minZoom = 5.0f; //min orthografic size of camera lense

    [SerializeField] Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float playerXPosition = PlayerGameObject.transform.position.x;
        if (playerXPosition < 35)
        {
            cam.transform.position = new Vector3(playerXPosition, yOffset, -10);

            cam.orthographicSize = (playerXPosition * zoomSpeed) + minZoom;

            /*
            if (yOffset < 1.5)
            {
                yOffset += playerXPosition * .01f * Time.deltaTime;
            }
            */
        //}
        
        /*
        else
        {
            float playerYPosition = PlayerGameObject.transform.position.y;

            cam.transform.position = new Vector3(playerXPosition, playerYPosition + 3, -10);

        }

    }
    */
}
