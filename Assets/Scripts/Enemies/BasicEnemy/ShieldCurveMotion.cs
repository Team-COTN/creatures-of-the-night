using System;
using Unity.VisualScripting;
using UnityEngine;

public class ShieldCurvedMotion : MonoBehaviour
{
    [SerializeField] private Transform origin;
    public float radius = 0.5f;
    public float rotationSpeed = 60f;
    float timer; //if == duration, pingpong
    public float duration = 1f; //of movement before it pingpongs back
    int direction = 1;


    void Start()
    {

    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        float lerpDuration = 0f;
        float realTime = 0f;
        timer += Time.deltaTime;

        if (timer >= duration)
        {
            direction *= -1;
            timer = 0;

        }

        //rotationSpeed = Mathf.Lerp()
        transform.RotateAround(origin.position, Vector3.forward, rotationSpeed * direction * Time.deltaTime);
    }
}
