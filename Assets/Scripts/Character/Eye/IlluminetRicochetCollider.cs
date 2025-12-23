using Unity.VisualScripting;
using UnityEngine;

public class IlluminetRicochetCollider : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        //when entering, turn trigger OFF (making it solid)
        Debug.Log("Thing bumped");
        if (other.gameObject.layer == LayerMask.NameToLayer("NonComposite") && other.isTrigger)
        {
            Debug.Log("NonCompositE bumped");
            other.isTrigger = false;

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //when exiting, turn trigger ON (making it pass-through)
        if (other.gameObject.layer == LayerMask.NameToLayer("NonComposite") && !other.isTrigger)
        {
            Debug.Log("NonCompositE eexited");
            other.isTrigger = true;
        }
    }

    // void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.gameObject.layer == LayerMask.NameToLayer("NonComposite") && !other.enabled)
    //         other.enabled = true;
    // }
}
