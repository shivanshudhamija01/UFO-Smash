public abstract class BaseState<T>
{
    protected T controller;
    protected BaseState(T controller)
    {
        this.controller = controller;
    }

    public virtual void UpdateState() { }
    public virtual void OnEnterState() { }
    public virtual void OnExitState() { }
    public virtual void FixedUpdateState() { }
}
