using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlowingLight : MonoBehaviour
{
    private Transform playerTransform;
    [SerializeField] Light2D glowPointLight;
    private GameObject player;

    public float maxIntensity = 5f;
    public float minIntensity = 0f;
    public float range = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    // void OnTriggerEnter2D(Collider2D col)
    // {
    //playerTransform = player.transform;
    // if (col == player.Collider2D)


    public void OnTriggerEnter2D(Collider2D collision)
    {
        playerTransform = player.transform;
        Debug.Log("COL ENTR");

        if (collision.TryGetComponent(out Character character))
        {
            Debug.Log("PLAYER COL ENTR");
            float distance = Vector3.Distance(playerTransform.position, transform.position);
            float t = Mathf.Clamp01(1 - (distance / range));
            glowPointLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, 1);

        }
    }

}
