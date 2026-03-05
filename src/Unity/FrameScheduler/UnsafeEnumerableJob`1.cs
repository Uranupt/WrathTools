using System;
using System.Collections;
using System.Collections.Generic;


namespace WrathTools.Unity
{
  /// <summary>
  /// A FrameScheduler Job for iterating through a live IEnumerable when certain no mutations will occur.
  /// Proper behavior cannot be assured if mutation occurs.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public sealed class UnsafeEnumerableJob<T> : FrameScheduler.IJob
  {

    private readonly IEnumerable<T> _enumerable;
    private IEnumerator<T> _enumerator;
    private readonly Action<T> _onWork;
    private readonly Action _onDone;
    private readonly Func<T, bool> _onWorkFunc;
    private readonly bool _useFunc = false;

    public UnsafeEnumerableJob(IEnumerable<T> enumerable, Action<T> onWork, Action onDone = null)
    {
      _enumerable = enumerable;
      _onWork = onWork;
      _onDone = onDone;
    }

    public UnsafeEnumerableJob(IEnumerable<T> enumerable, Func<T, bool> onWork, Action onDone = null)
    {
      _enumerable = enumerable;
      _onWorkFunc = onWork;
      _onDone = onDone;
      _useFunc = true;
    }

    public bool DoWork()
    {
      _enumerator ??= _enumerable.GetEnumerator();
      if(_enumerator.MoveNext())
      {
        if(_useFunc)
        {
          return _onWorkFunc.Invoke(_enumerator.Current);
        }
        else
        {
          _onWork?.Invoke(_enumerator.Current);
          return true;
        } 
      }
      else
      {
        return false;
      }
    }

    public void FinishWork()
    {
      _enumerator?.Dispose();
      _onDone?.Invoke();
    }

  }
}