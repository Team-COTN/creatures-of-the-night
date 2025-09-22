using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class VisualProjectiles : MonoBehaviour, IPooledObject
{
    public float projectileSpeed;

    //could we attatch the projectiles as children of a different target?
    //or would the target even need to switch or just change positions???
    public void OnObjectSpawn()
    {
        //play grow animation
        //target = idle player target (need to animate) 
    }

    public void OnObjectLaunch()
    {
        //switch to launch target 
        //target = launch target
    }
    public void Update()
    {
        //follow target
        //position = target pos
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }
}
