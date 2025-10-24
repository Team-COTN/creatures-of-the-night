using UnityEngine;

public class CharacterSwitchDashState : CharacterState
{
    private bool EyeWasPressedThisFrame => InputManager.GetEyeWasPressedThisFrame();

    public CharacterSwitchDashState(Character character) : base(character)
    {
    }

    private bool IsFinishedSwitchDashing => _character.SwitchDashTimer <= 0;
    private Vector2 _dashDirection;

    public override void StateEnter()
    {
        base.StateEnter();
        _character.InitializeSwitchDashTimer();
        _dashDirection = _character.IsFacingRight ? Vector2.left : Vector2.right;
        _character.StateChangeEvent(_character, "SwitchDash");
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        if (IsFinishedSwitchDashing)
        {
            _stateMachine.ChangeState(_character.IdleState);
        }
        else if (EyeWasPressedThisFrame) _stateMachine.ChangeState(_character.EyeState);

    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        _character.Dash(_dashDirection, _movementData.switchdashDistance/_movementData.switchdashDuration);
    }

    //can only be accessed in the Dash State
    //can only be activated within .5 seconds of the Dash State being activated
    //when in dash state, and H button pressed,
    //^change direction and increase the magnitude of velocity


    //if switchdashTimer > 0, the duration is continuing, continue in the Switchdash state. 
    //if switchdashTimer > 0, end switchdash. 

    //if switchdash COOLdown > 0, the dash just started and character may Enter the Switchdash state. 

}
