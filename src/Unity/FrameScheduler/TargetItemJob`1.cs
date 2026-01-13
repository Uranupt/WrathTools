using System;
using System.Collections.Generic;


namespace WrathTools.Unity
{
  public sealed class TargetItemJob<T> : FrameScheduler.IJob where T : IEquatable<T>
  {

    //TODO: Handle or scrap Behaviors

    private readonly OverflowOption _overflow;
    //private readonly ItemBehaviors _behaviors;
    private readonly IEnumerable<T> _enumerable;
    private readonly Func<T, bool> _onWork;
    private readonly Action _onDone;
    private readonly int _steps;

    //private T _lastItem;

    public TargetItemJob(
      IEnumerable<T> enumerable,
      Func<T, bool> onWork,
      Action onDone = null,
      int steps = 1,
      OverflowOption overflow = OverflowOption.EndWork
    )
    {
      _enumerable = enumerable;
      _onWork = onWork;
      _onDone = onDone;
      _steps = Math.Max(steps, 0);
      _overflow = overflow;
      //_behaviors = behaviors;
    }

    //private bool NullCheck(T value) => !value.Equals(null) || _behaviors.HasFlag(ItemBehaviors.AllowNull);
    //private bool DuplicateCheck(T value) => !value.Equals(_lastItem) || !_behaviors.HasFlag(ItemBehaviors.SkipDuplicates);

    public bool DoWork()
    {
      IEnumerator<T> enumerator = _enumerable.GetEnumerator();
      T currentItem = default;
      int stepsTaken = 0;
      while(stepsTaken < _steps)
      {
        if(enumerator.MoveNext())
        {
          currentItem = enumerator.Current;
        }
        else
        {
          if(stepsTaken == 0)
          {
            enumerator.Dispose();
            return false;
          }
          switch(_overflow)
          {
            case OverflowOption.EndWork:
            {
              enumerator.Dispose();
              return false;
            }
            case OverflowOption.Clamp:
            {
              enumerator.Dispose();
              return _onWork.Invoke(currentItem);
            }
            case OverflowOption.Wrap:
            {
              enumerator.Reset();
              break;
            }
            case OverflowOption.UseDefault:
            {
              enumerator.Dispose();
              return _onWork.Invoke(default);
            }
          }
        }
        stepsTaken++;
      }
      return _onWork.Invoke(currentItem);
    }

    public void FinishWork()
    {
      _onDone?.Invoke();
    }

  }
}