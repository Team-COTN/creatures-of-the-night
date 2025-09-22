using System;
using UnityEngine;

public class CharacterWalkState : CharacterState
{
    public CharacterWalkState(Character character) : base(character)
    {
    }

    private bool IsFalling => !_character.IsGrounded;
    private bool IsStopped => Mathf.Abs(InputManager.GetMovement().x) < _movementData.moveThreshold;
    private bool JumpInputPressed => InputManager.GetJumpWasPressedThisFrame();
    private bool SlashInputPressed => InputManager.GetSlashWasPressedThisFrame();
    private bool CanJump => _character.CanJump();
    private bool DashInputPressed => InputManager.GetDashWasPressedThisFrame();
    private bool CanDash => _character.DashCooldown <= 0;
    
    public override void StateEnter()
    {
        _character.StateChangeEvent(_character, "Move");
    }
    public override void StateUpdate()
    {
        base.StateUpdate();
        if (DashInputPressed && CanDash) _stateMachine.ChangeState(_character.DashState);
        else if (IsFalling) _stateMachine.ChangeState(_character.AirState);
        else if (JumpInputPressed && CanJump) _stateMachine.ChangeState(_character.JumpState);
        else if (SlashInputPressed) _stateMachine.ChangeState(_character.SlashState);
        else if (IsStopped) _stateMachine.ChangeState(_character.IdleState);
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        _character.Move(InputManager.GetMovement(), _movementData.groundAcceleration, _movementData.groundDeceleration);
    }
}
