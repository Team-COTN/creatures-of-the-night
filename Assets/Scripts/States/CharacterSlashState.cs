using UnityEngine;

public class CharacterSlashState : CharacterState
{
    private bool EyeWasPressedThisFrame => InputManager.GetEyeWasPressedThisFrame();

    public CharacterSlashState(Character character) : base(character)
    {
    }
    
    //variables

    
    public override void StateEnter()
    {
        base.StateEnter();
        _character.Slash();
        _stateMachine.ChangeState(_character.IdleState);
        _character.StateChangeEvent(_character, "Slash");
    }
    public override void StateUpdate()
    {
        if (EyeWasPressedThisFrame) _stateMachine.ChangeState(_character.EyeState);
    }

}