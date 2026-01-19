using System;


namespace WrathTools
{
  public sealed class ValueScope<T> : IStronglyCommittableScope
  {

    private Action<T> _setter;
    private T _initialValue;

    public bool Active { get; private set; }
    public bool Committed { get; private set; }
    public bool CanCommit => Active;

    public ValueScope(Action<T> setter, T currentValue, T tempValue)
    {
      _setter = setter;
      _initialValue = currentValue;
      _setter?.Invoke(tempValue);
      Active = true;
    }

    public void Exit()
    {
      if(!Active) { return; }
      Active = false;
      if(Committed) { return; }
      _setter?.Invoke(_initialValue);
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