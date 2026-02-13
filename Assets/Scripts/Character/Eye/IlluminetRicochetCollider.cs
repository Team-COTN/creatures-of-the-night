using System;
using Unity.VisualScripting;
using UnityEngine;

public class IlluminetRicochetCollider : MonoBehaviour
{

    private Collider2D _collider;
    void OnTriggerEnter2D(Collider2D other)
    {
        //when entering, turn trigger OFF (making it solid)
        if (other.gameObject.layer == LayerMask.NameToLayer("NonComposite") && other.isTrigger)
        {            
            _collider = other;
            other.isTrigger = false;
            other.GetComponent<GlowOutline>().Glow();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //when exiting, turn trigger ON (making it pass-through)
        if (other.gameObject.layer == LayerMask.NameToLayer("NonComposite") && !other.isTrigger)
        {
            other.isTrigger = true;
            other.GetComponent<GlowOutline>().Dim();

        }
    }

    private void OnDisable()
    {   
        _collider.isTrigger = true;
        _collider.GetComponent<GlowOutline>().Dim();
    }

}
