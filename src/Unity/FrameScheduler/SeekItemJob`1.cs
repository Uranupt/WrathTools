using System;
using System.Collections.Generic;


namespace WrathTools.Unity
{
  public sealed class SeekItemJob<T> : FrameScheduler.IJob
  {

    private readonly IEnumerable<T> _enumerable;
    private readonly Action<T> _onWork;
    private readonly Func<T, bool> _onWorkFunc;
    private readonly bool _useFunc = false;
    private readonly Action _onDone;
    private readonly Func<T, bool> _predicate;

    public SeekItemJob(
      IEnumerable<T> enumerable,
      Action<T> onWork,
      Func<T, bool> predicate,
      Action onDone = null
    )
    {
      _enumerable = enumerable;
      _onWork = onWork;
      _predicate = predicate;
      _onDone = onDone;
    }

    public SeekItemJob(
      IEnumerable<T> enumerable,
      Func<T, bool> onWork,
      Func<T, bool> predicate,
      Action onDone = null
    )
    {
      _enumerable = enumerable;
      _onWorkFunc = onWork;
      _predicate = predicate;
      _onDone = onDone;
      _useFunc = true;
    }

    public bool DoWork()
    {
      IEnumerator<T> enumerator = _enumerable.GetEnumerator();
      while(enumerator.MoveNext())
      {
        T item = enumerator.Current;
        if(_predicate.Invoke(item))
        {
          enumerator.Dispose();
          if(_useFunc)
          {
            return _onWorkFunc.Invoke(item);
          }
          else
          {
            _onWork.Invoke(item);
            return true;
          }
        }
      }
      return false;
    }

    public void FinishWork()
    {
      _onDone?.Invoke();
    }

  }
}