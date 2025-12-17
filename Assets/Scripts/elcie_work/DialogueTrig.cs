using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueTrig : MonoBehaviour
{
    public bool DialogueWasPressedThisFrame => InputManager.GetDialogueWasPressedThisFrame();
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
    public void TriggerDialogueOff()
    {
        /*
        if (GetDialogueWasPressedThisFrame)
        {   
            //if dialogue is not done, make it play the next sentence
            //if it is the last sentence,
                //make it go away
                // FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
            //     if (InputManager.GetJumpWasPressedThisFrame())
            // {
            //     Debug.Log("got input");
            // }
        }
        */
    }
}
