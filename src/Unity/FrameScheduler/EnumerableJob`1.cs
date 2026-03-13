using System;
using System.Collections.Generic;


namespace WrathTools.Unity
{
  /// <summary>
  /// A FrameScheduler Job for iterating through a copy of an IEnumerable to allow for mutation during iteration.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public sealed class EnumerableJob<T> : FrameScheduler.IJob
  {

    private readonly List<T> _list;
    private IEnumerator<T> _enumerator;
    private readonly Action<T> _onWork;
    private readonly Func<T, bool> _onWorkFunc;
    private readonly bool _useFunc = false;

    public event Action OnDone;

    public EnumerableJob(IEnumerable<T> enumerable, Action<T> onWork)
    {
      _list = new List<T>(enumerable);
      _onWork = onWork;
    }

    public EnumerableJob(IEnumerable<T> enumerable, Func<T, bool> onWork)
    {
      _list = new List<T>(enumerable);
      _onWorkFunc = onWork;
      _useFunc = true;
    }

    public bool DoWork()
    {
      _enumerator ??= _list.GetEnumerator();
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
      OnDone?.Invoke();
    }

  }
}