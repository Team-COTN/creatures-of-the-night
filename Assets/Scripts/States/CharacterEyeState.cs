using UnityEngine;

public class CharacterEyeState : CharacterState
{
    public CharacterEyeState(Character character) : base(character)
    {
    }
    public override void StateEnter()
    {
        //making idle for now to trigger the idle animation. May add eye state for event purposes. 
        _character.StateChangeEvent(_character, "Idle");
    }

    private bool EyeWasPressedThisFrame => InputManager.GetEyeWasPressedThisFrame();
    private bool enableEyeState = false;

    public override void StateUpdate()
    {
        base.StateUpdate();
        if (EyeWasPressedThisFrame) _stateMachine.ChangeState(_character.IdleState);
    }


}