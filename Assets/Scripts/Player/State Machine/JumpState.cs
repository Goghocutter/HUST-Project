public class JumpState : BaseState
{
    private bool leftGround;
    private float t;
    private float groundedHold;

    private const float FailSafeAir = 0.35f;     
    private const float MaxAirTime = 2.0f;      
    private const float GroundHyst = 0.06f;    
    public override void Enter(PlayerContext ctx)
    {
        leftGround = false;
        t = 0f;
        groundedHold = 0f;

        if (ctx.motor != null)
            ctx.motor.wantJumpPulse = true;
    }

    public override void Tick(PlayerContext ctx, float dt)
    {
        t += dt;

        if (ctx.motor != null)
            ctx.motor.wantSprint = ctx.SprintHeld;

        bool groundedNow = ctx.IsGrounded || ctx.IsGroundedProbing(0.12f);

        if (!leftGround && !groundedNow)
            leftGround = true;

        if (leftGround && groundedNow) groundedHold += dt;
        else groundedHold = 0f;
    }

    public override IPlayerState TryTransition(PlayerContext ctx)
    {
        if (!leftGround && t >= FailSafeAir)
            return NextGroundState(ctx);

        if (leftGround && groundedHold >= GroundHyst)
            return NextGroundState(ctx);

        if (t >= MaxAirTime)
            return NextGroundState(ctx);

        return null;
    }

    private IPlayerState NextGroundState(PlayerContext ctx)
    {
        if (ctx.CrouchHeld) return new CrouchState();
        return (ctx.Move.sqrMagnitude > 0.01f) ? new WalkRunState() : new IdleState();
    }
}
