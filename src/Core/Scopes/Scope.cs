using System;


namespace WrathTools
{
  public sealed class Scope : IStronglyCommittableScope
  {

    private readonly Action _onExit;

    public bool Active { get; private set; }
    public bool Committed { get; private set; }
    public bool CanCommit => Active;

    public Scope(Action onEnter, Action onExit)
    {
      onEnter?.Invoke();
      _onExit = onExit;
      Active = true;
    }

    public void Exit()
    {
      if(!Active) { return; }
      Active = false;
      if(Committed) { return; }
      _onExit?.Invoke();
    }

    public void Dispose() => Exit();

    public void Commit()
    {
      if(!Active) { return; }
      Committed = true;
    }

    bool ICommittableScope.Commit()
    {
      Commit();
      return Committed;
    }

  }
}