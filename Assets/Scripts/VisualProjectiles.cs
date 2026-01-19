using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class VisualProjectiles : MonoBehaviour
{
    private float transitionSpeed = 60f;
    public Transform target;

    private void OnDisable() { target = null; }
    public void SetTarget(Transform t) { target = t; }
    
    private void Update()
    {
        if (target != null)
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * transitionSpeed);
    }
}