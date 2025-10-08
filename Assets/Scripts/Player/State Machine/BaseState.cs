public abstract class BaseState : IPlayerState
{
    public virtual void Enter(PlayerContext ctx) { }
    public virtual void Exit(PlayerContext ctx) { }
    public virtual void Tick(PlayerContext ctx, float dt) { }
    public virtual IPlayerState TryTransition(PlayerContext ctx) => null;
}
