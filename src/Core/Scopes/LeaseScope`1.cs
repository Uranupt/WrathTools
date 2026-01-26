using System;


namespace WrathTools
{
  public sealed class LeaseScope<T> : ICommittableScope
  {

    private readonly bool _canKeep;
    private readonly Action<T> _onReturn;

    public bool Committed { get; private set; }
    public bool Active { get; private set; }
    public T Value { get; private set; }

    public bool CanCommit => Active && _canKeep;

    public LeaseScope(T value, Action<T> onReturn, bool canKeep = false)
    {
      Value = value;
      _onReturn = onReturn;
      _canKeep = canKeep;
    }

    public bool Keep() => Commit();

    public bool Commit()
    {
      if(CanCommit)
      {
        Committed = true;
      }
      return Committed;
    }

    public void Dispose() => Exit();

    public void Exit()
    {
      if(!Active) { return; }
      Active = false;
      if(Committed) { return; }
      _onReturn?.Invoke(Value);
      Value = default;
    }
  }
}