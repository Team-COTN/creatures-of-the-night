using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class NPC : MonoBehaviour
{
    DialogueManager dialogueScript;
    public Dialogue dialogue;
    public bool triggerDialogue = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Start Dia?");

        if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out Character character))
        {
            Debug.Log("StartDialogue");
            FindFirstObjectByType<DialogueManager>().StartDialogue(dialogue);

        }
    }
}
