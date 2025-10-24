public class CharacterJumpParryState : CharacterState
{
    private bool EyeWasPressedThisFrame => InputManager.GetEyeWasPressedThisFrame();

    public CharacterJumpParryState(Character character) : base(character)
    {
    }
    
    public override void StateEnter()
    {
        base.StateEnter();
        _character.JumpParry();
        _stateMachine.ChangeState(_character.AirState);
        _character.StateChangeEvent(_character, "JumpParry");
    }
    public override void StateUpdate()
    {
        if (EyeWasPressedThisFrame) _stateMachine.ChangeState(_character.EyeState);
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        _character.Move(InputManager.GetMovement(), _movementData.airHorizontalAcceleration, _movementData.airHorizontalDeceleration);
    }
}
