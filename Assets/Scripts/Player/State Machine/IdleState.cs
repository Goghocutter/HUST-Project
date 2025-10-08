using UnityEngine.InputSystem.LowLevel;

public class IdleState : BaseState
{
    public override void Enter(PlayerContext ctx)
    {
        ctx.Cache();
        if (ctx.motor == null) return;
        ctx.motor.wantSprint = false;
        ctx.motor.wantCrouch = false;
    }

    public override void Tick(PlayerContext ctx, float dt)
    {
        ctx.motor.wantSprint = false;
        ctx.motor.wantCrouch = ctx.CrouchHeld;
    }

    public override IPlayerState TryTransition(PlayerContext ctx)
    {
        if (ctx.JumpPressed && (ctx.IsGrounded || ctx.IsCoyoteGrounded(0.2f)))
            return new JumpState();

        if (ctx.CrouchHeld) return new CrouchState();
        if (ctx.Move.sqrMagnitude > 0.01f) return new WalkRunState();
        return null;
    }
}
