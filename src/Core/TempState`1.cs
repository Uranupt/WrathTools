using System;


namespace WrathTools
{
  public sealed class TempState<T> : IDisposable
  {

    private Action<T> _setter;
    private T _initialValue;
    public bool DontRevert;

    public TempState(Action<T> setter, T currentValue, T tempValue)
    {
      _setter = setter;
      _initialValue = currentValue;
      _setter?.Invoke(tempValue);
    }

    public void Dispose()
    {
      if(DontRevert) { return; }
      _setter?.Invoke(_initialValue);
    }

  }
}