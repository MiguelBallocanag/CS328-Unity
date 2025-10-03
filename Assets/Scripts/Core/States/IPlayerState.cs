public interface IPlayerState
{
    void Enter(PlayerController pc);
    void Tick();
    void Exit();
}