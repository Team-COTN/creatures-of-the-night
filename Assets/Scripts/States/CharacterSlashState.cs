using UnityEngine;

public class CharacterSlashState : CharacterState
{
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
}