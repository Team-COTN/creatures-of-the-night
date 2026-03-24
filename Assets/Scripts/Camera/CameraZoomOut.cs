using UnityEngine;
using System.Collections;

public class CameraZoomOut : MonoBehaviour
{

    [Header("Detect Cluster")]
    public GameObject[] clusterObjects;
    private bool[] counted;
    private int counter = 0;
    public CameraZone cameraZone;
    private float zoomOutTimer = 5.0f;
    private IEnumerator coroutine;
    public Light2D light2D1;
    public Light2D light2D2;
    public Light2D light2D3;

    void Start() 
    {
        counted = new bool[clusterObjects.Length];
    }

    IEnumerator WaitForUnzoom(float wait)
    {
        // suspend execution for 'wait' seconds
        yield return new WaitForSeconds(wait);
        print("UnzOOOOO0000m " + Time.time);
        cameraZone.SetPriority(3);
    }

    void Update()
    {
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
            Debug.Log("SetPriority(10)");
            cameraZone.SetPriority(10);

            coroutine = WaitForUnzoom(7.0f);
            StartCoroutine(coroutine);
            counter = clusterObjects.Length + 1;
        }
    }
}

