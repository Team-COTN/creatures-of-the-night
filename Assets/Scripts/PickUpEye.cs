using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpEye : MonoBehaviour
{
    /*
    [SerializeField] GameObject player;
    [SerializeField] GameObject eyeForCollecting;
    [SerializeField] GameObject tempLight;


    void Start()
    {
        eyeForCollecting.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string collectableTag = this.gameObject.tag;

        if (other.gameObject == player)
        {
            switch (collectableTag)
            {
                case "Eye":
                    {
                        other.gameObject.GetComponent<PlayerControls>().eyeCollected = true;
                        Debug.Log("Eye Collected State: " + other.gameObject.GetComponent<PlayerControls>().eyeCollected);
                        eyeForCollecting.SetActive(true);
                        tempLight.SetActive(false);
                        break;
                    }
                default:
                    {
                        Debug.Log("Did not recognize tag");
                        break;
                    }
            }
            this.gameObject.SetActive(false);

        }
    }
    */
}