using System;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public Animator winAnimator;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out Character character))
        {
            Debug.Log("Winnnnnnnn");
            winAnimator.SetTrigger("Win");
        }
    }

}
