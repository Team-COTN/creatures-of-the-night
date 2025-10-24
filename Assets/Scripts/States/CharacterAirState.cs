using UnityEngine;

public class CharacterAirState : CharacterState
{
    public CharacterAirState(Character character) : base(character)
    {
    }

    private bool IsDescending => _character.VerticalVelocity < 0;
    private bool IsGrounded => _character.IsGrounded;
    private bool JumpInputPressed => InputManager.GetJumpWasPressedThisFrame();
    private bool JumpInputReleased => InputManager.GetJumpWasReleasedThisFrame() || _character.JumpReleasedDuringBuffer;
    private bool CanJump => _character.CanJump();
    private bool CanJumpParry => _character.CanJumpParry();

    private bool DashInputPressed => InputManager.GetDashWasPressedThisFrame();
    private bool CanDash => _character.DashCooldown <= 0;
    private bool EyeWasPressedThisFrame => InputManager.GetEyeWasPressedThisFrame();

    public override void StateEnter()
    {
        _character.StateChangeEvent(_character, "Air");
    }
    public override void StateUpdate()
    {
        base.StateUpdate();
        if (DashInputPressed && CanDash) _stateMachine.ChangeState(_character.DashState);
        else if (JumpInputPressed && CanJumpParry) _stateMachine.ChangeState(_character.JumpParryState);
        else if (IsDescending && IsGrounded)
        {
            _character.Land();
            _stateMachine.ChangeState(_character.IdleState);
        }
        else if (JumpInputReleased && _character.IsJumping) _character.CancelJumpEarly();
        else if (EyeWasPressedThisFrame) _stateMachine.ChangeState(_character.EyeState);
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        _character.AirPhysics();
        _character.Move(InputManager.GetMovement(), _movementData.airHorizontalAcceleration, _movementData.airHorizontalDeceleration);
    }
}
