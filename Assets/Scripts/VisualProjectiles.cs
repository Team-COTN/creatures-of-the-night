using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

//this is attached to both Projectiles. BUT! They both need different offsets
public class VisualProjectiles : MonoBehaviour, IPooledObject
{
    public float transitionSpeed;
    public GameObject idleTarget;
    public GameObject shootTarget;
    private Vector2 target;
    public float Xoffset;
    public float Yoffset;
    
    //OnObjectSpawn
    public void OnEnable()
    {
        //will need to add offset to both (in inspector -> different for Small vs Large)
        target = new Vector2(idleTarget.transform.position.x + Xoffset, idleTarget.transform.position.y + Yoffset);
        
        Debug.Log(idleTarget.transform.position);
        
        //play grow animation
        //target = idle player target (need to animate) 
    }

    public void OnObjectLaunch()
    {
        target = shootTarget.GameObject().transform.position;

        //switch to launch target 
        //target = launch target
    }
    public void Update()
    {
        target = new Vector2(idleTarget.transform.position.x + Xoffset, idleTarget.transform.position.y + Yoffset);

        //here temporarily since target is not getting assigned OnObjectLaunch() (not getting called yet)
        // target.position = shootTarget.GameObject().transform.position;

        // Smoothly move toward the target
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * transitionSpeed);
    }
}