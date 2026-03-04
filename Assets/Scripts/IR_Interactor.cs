using UnityEngine;
using UnityEngine.VFX;

public class IR_Interactor : MonoBehaviour
{
    [SerializeField] public VisualEffect vfx;

    void OnCollisionEnter2D(Collision2D _collider)
    {
        if (_collider.gameObject.layer == LayerMask.NameToLayer("NonComposite"))
        {
            CollisionFX();
            this.gameObject.SetActive(false);
        }
    }

    public void CollisionFX()
    {
        VisualEffect newVFX = Instantiate(vfx, transform.position, transform.rotation).GetComponent<VisualEffect>();
        newVFX.SendEvent("Hit");
    }
}
