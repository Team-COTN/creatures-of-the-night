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
    public EventGlow eventGlow1;
    public EventGlow eventGlow2;
    public EventGlow eventGlow3;

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
    IEnumerator WaitToGlow(float wait, EventGlow glow)
    {
        // suspend execution for 'wait' seconds
        yield return new WaitForSeconds(wait);
        glow.Glow();
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
            StartCoroutine(WaitToGlow(2f, eventGlow1));
            StartCoroutine(WaitToGlow(3.5f, eventGlow2));
            StartCoroutine(WaitToGlow(5f, eventGlow3));
            coroutine = WaitForUnzoom(5.5f);
            StartCoroutine(coroutine);
            counter = clusterObjects.Length + 1;
        }
    }
}

