using UnityEngine;

public class NPC : MonoBehaviour
{
    DialogueManager dialogueScript;
    Dialogue dialogue;
    public bool triggerDialogue = false;

    private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.attachedRigidbody.TryGetComponent(out Character character))
            {
                FindObjectOfType<DialogueManager>().StartDialogue(dialogue);

                // dialogueScript.StartDialogue;
            }
        }
}
