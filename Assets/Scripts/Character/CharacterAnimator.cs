using System;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private Character character;
    private GameObject playerVisual;
    private Animator animator;

    private void Awake()
    {
        character = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        playerVisual = this.gameObject;
        animator = playerVisual.GetComponent<Animator>();
    }

    void Animate(string stateName)
    {
        bool jumping = false;
        if (stateName == "Jump")
        {
            animator.SetTrigger("Jump");
            jumping = true;
        }
        else if (stateName == "Move")
        {
            animator.SetTrigger("Move");
        }
        else if (stateName == "JumpParry")
        {
            animator.SetTrigger("JumpParry");
        }
        else if (stateName == "Air" && !jumping)
        {
            animator.SetTrigger("Air");
        }
        else if (stateName == "Slash")
        {
            animator.SetTrigger("Slash");
        }
        else if (stateName == "Idle" && (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Animation")))
        {
            animator.SetTrigger("Idle");
        }

        // if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        // {
        //     jumping = false;
        // }
    }
    
    private void OnEnable()
    {
        character.AddCharacterStateObserver(Animate);
    }
    private void OnDisable()
    {
        character.RemoveCharacterStateObserver(Animate);
    }

    public void PlayAnimation()
    {
        Debug.Log("Play Animation");
    }

}
