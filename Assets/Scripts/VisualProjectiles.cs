using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class VisualProjectiles : MonoBehaviour
{
    private GameObject character;
    private float transitionSpeed = 60f;
    private Quaternion rotate;
    private Transform target;
    public bool facingRight;

    // Quaternion.Euler(0f, facingRight ? 0f : 180f, 0f);

    private void Awake()
    {
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>().gameObject;
    }
    
    private void OnDisable() { target = null; }
    public void SetTarget(Transform t) { target = t; }
    public void SetTarget(Transform t, float r) 
    { 
        target = t; 
        rotate = Quaternion.Euler(0f, facingRight ? 0f : 180f, r); 
    }
    
    private void Update()
    {
        // facingRight = character.IsFacingRight();

        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * transitionSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * transitionSpeed);
        }
    }
}