public abstract class BaseState : IPlayerState {
    protected PlayerController pc;

    public virtual void Enter(PlayerController p)
    {
        pc = p;
    }
    public virtual void Tick() { } 
    public virtual void Exit() { }   
}
