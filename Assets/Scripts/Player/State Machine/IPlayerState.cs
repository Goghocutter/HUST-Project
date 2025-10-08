public interface IPlayerState
{
    void Enter(PlayerContext ctx);
    void Exit(PlayerContext ctx);
    void Tick(PlayerContext ctx, float dt);
    IPlayerState TryTransition(PlayerContext ctx);
}
