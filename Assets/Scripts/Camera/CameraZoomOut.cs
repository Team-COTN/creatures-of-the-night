using UnityEngine;

public class CameraZoomOut : MonoBehaviour
{

    [Header("Detect Cluster")]
    public GameObject[] clusterObjects;
    private bool[] counted;
    private int counter = 0;

    void Start() 
    {
        counted = new bool[clusterObjects.Length];
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
            Debug.Log("cluster Complete");
            
        }
    }
}

