using Unity.VisualScripting;
using UnityEngine;

public class IlluminetRicochetCollider : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("NonComposite") && other.enabled)
            other.enabled = false;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("NonComposite") && !other.enabled)
            other.enabled = true;
    }
}
