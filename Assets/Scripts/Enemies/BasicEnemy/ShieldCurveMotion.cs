using System;
using Unity.VisualScripting;
using UnityEngine;

public class ShieldCurvedMotion : MonoBehaviour
{
    [SerializeField] private Transform origin;
    public float radius = 0.5f;
    private float currentRotationSpeed;
    public float maxRotSpeed = 60f;
    public float minRotSpeed = 5f;
    public float slowingFactor = 1f;
    float timer; //if == duration, pingpong
    public float duration = 1f; //of movement before it pingpongs back
    int direction = 1;


    void Start()
    {
        currentRotationSpeed = maxRotSpeed;
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        /*timer += Time.deltaTime;
        currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0f, slowingFactor * Time.deltaTime);

        if (timer >= duration)
        {
            direction *= -1;
            timer = 0;
            currentRotationSpeed = maxRotSpeed;

        }

        transform.RotateAround(origin.position, Vector3.forward, currentRotationSpeed * direction * Time.deltaTime);
        */

        timer += Time.deltaTime;

        if (timer >= duration)
        {
            direction *= -1;
            timer = 0f;
        }

        float t_pingpong = Mathf.PingPong(timer, duration);
        float t_normalized = t_pingpong / duration;
        float smoothFactor = Mathf.SmoothStep(0f, 1f, t_normalized);
        float speedFactor = 1f - Mathf.Abs(t_normalized - 0.5f) * 2f;

        float currentRotationSpeed = Mathf.Lerp(minRotSpeed, maxRotSpeed, speedFactor);

        transform.RotateAround(origin.position, Vector3.forward, currentRotationSpeed * Time.deltaTime * direction);
    }
}
