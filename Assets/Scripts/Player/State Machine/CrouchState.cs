public class CrouchState : BaseState
{
    public override void Enter(PlayerContext ctx)
    {
        if (ctx.motor == null) return;
        ctx.motor.wantCrouch = true;
        ctx.motor.wantSprint = false;
    }

    public override void Tick(PlayerContext ctx, float dt)
    {
        ctx.motor.wantCrouch = true;
        ctx.motor.wantSprint = false;
    }

    public override IPlayerState TryTransition(PlayerContext ctx)
    {
        if (!ctx.CrouchHeld)
            return (ctx.Move.sqrMagnitude > 0.01f) ? new WalkRunState() : new IdleState();

        if (ctx.JumpPressed && (ctx.IsGrounded || ctx.IsCoyoteGrounded(0.2f)))
            return new JumpState();

        return null;
    }
}
