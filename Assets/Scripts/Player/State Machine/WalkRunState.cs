using UnityEngine.InputSystem.LowLevel;

public class WalkRunState : BaseState
{
    public override void Enter(PlayerContext ctx)
    {
        if (ctx.motor == null) return;
        ctx.motor.wantCrouch = false;
    }

    public override void Tick(PlayerContext ctx, float dt)
    {
        ctx.motor.wantSprint = ctx.SprintHeld;
        ctx.motor.wantCrouch = false;
    }

    public override IPlayerState TryTransition(PlayerContext ctx)
    {
        if (ctx.CrouchHeld) return new CrouchState();
        if (ctx.JumpPressed && (ctx.IsGrounded || ctx.IsCoyoteGrounded(0.2f)))
            return new JumpState();

        if (ctx.Move.sqrMagnitude <= 0.01f) return new IdleState();
        return null;
    }
}
