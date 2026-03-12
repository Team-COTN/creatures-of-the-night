using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;
    private RuntimeAnimatorController originalController;

    private void Awake() => originalController = animator.runtimeAnimatorController;

    public void PlayIdle() => animator.Play("Idle");
    public void PlayWalk() => animator.Play("Walk");
    public void PlayJump() => animator.Play("Jump");
    public void PlayFall() => animator.Play("Fall");
    public void PlayJumpParry() => animator.Play("Jump Parry");
    public void PlayDash() => animator.Play("Dash");
    public void PlaySwitchDash() => animator.Play("Switch-Dash");
    public void PlayAirDash() => animator.Play("Air Dash");
    public void PlayAirSwitchDash() => animator.Play("Air Switch-Dash");
    public void PlaySlash() => animator.Play("Slash");

    public void PlayClip(AnimationClip clip)
    {
        var overrideController = new AnimatorOverrideController(originalController);
        animator.runtimeAnimatorController = overrideController;
        overrideController["Cinematic"] = clip;
        animator.Play("Cinematic");
    }

    public void StopClip() => animator.runtimeAnimatorController = originalController;

    public bool IsClipFinished()
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        return info.normalizedTime >= 1f && !info.loop;
    }
}