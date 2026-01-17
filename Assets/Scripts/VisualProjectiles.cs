using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class VisualProjectiles : MonoBehaviour
{
    public float transitionSpeed;
    public Transform target;
    
    //OnObjectSpawn
    public void OnEnable()
    {
        // play grow animation
        // target = idle player target (need to animate) 
    }
    
    // public void OnDisable() { objectLaunched = false; }

    public void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * transitionSpeed);
    }
}