using System;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private GameObject playerVisual;
    private Animator animator;

    private void Awake()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        //animator = player.GetComponentInChildren<Animator>();
        
        playerVisual = this.gameObject;
        animator = playerVisual.GetComponent<Animator>();
    }

    void Animate(string stateName)
    {
        if (stateName == "Idle" && (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Animation")))
        {
            animator.SetTrigger("Idle");
        }
        else if (stateName == "Move")
        {
            animator.SetTrigger("Move");
        }
        else if (stateName == "Jump")
        {
            animator.SetTrigger("Jump");
        }
        else if (stateName == "JumpParry")
        {
            animator.SetTrigger("JumpParry");
        }
        else if (stateName == "Slash")
        {
            animator.SetTrigger("Slash");
        }
    }
    
    private void OnEnable()
    {
        Character.AddCharacterStateObserver(Animate);
    }
    private void OnDisable()
    {
        Character.RemoveCharacterStateObserver(Animate);
    }

    public void PlayAnimation()
    {
        Debug.Log("Play Animation");
    }

}
