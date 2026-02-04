
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Animator animator;
    private Queue<string> sentences;
    public bool isTalking;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(NPC_SO_Base npcSoBase)
    {
        isTalking = true;
        animator.SetBool("isOpen", true);
        nameText.text = npcSoBase.dialogue.name;
        sentences.Clear();

        //remember the fix! I can use the sentences from the dialogue class with npcSoBase.dialogue.sentences. twice removed derivation!
        foreach (string sentence in npcSoBase.dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        isTalking = false;
        animator.SetBool("isOpen", false);
    }
}
