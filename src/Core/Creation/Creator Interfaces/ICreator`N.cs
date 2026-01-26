

namespace WrathTools
{

  //1
  public interface ICreator<out TResult> 
    : ICreatorWithoutArgs, ICreatorFor<TResult>
  {
    new TResult Create();
  }

  //2
  public interface ICreator<in T, out TResult> 
    : ICreatorWithArgs<T>, ICreatorFor<TResult>
  {
    new TResult Create(T arg);
  }

  //3
  public interface ICreator<in T1, in T2, out TResult> 
    : ICreatorWithArgs<T1, T2>, ICreatorFor<TResult>
  {
    new TResult Create(T1 arg1, T2 arg2);
  }

  //4
  public interface ICreator<in T1, in T2, in T3, out TResult> 
    : ICreatorWithArgs<T1, T2, T3>, ICreatorFor<TResult>
  {
    new TResult Create(T1 arg1, T2 arg2, T3 arg3);
  }

  //5
  public interface ICreator<in T1, in T2, in T3, in T4, out TResult> 
    : ICreatorWithArgs<T1, T2, T3, T4>, ICreatorFor<TResult>
  {
    new TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
  }

  //6
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, out TResult> 
    : ICreatorWithArgs<T1, T2, T3, T4, T5>, ICreatorFor<TResult>
  {
    new TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
  }

  //7
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, out TResult> 
    : ICreatorWithArgs<T1, T2, T3, T4, T5, T6>, ICreatorFor<TResult>
  {
    new TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
  }

  //8
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out TResult> 
    : ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7>, ICreatorFor<TResult>
  {
    new TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
  }

  //9
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out TResult>
    : ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8>, ICreatorFor<TResult>
  {
    new TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
  }

  //10
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, out TResult>
    : ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9>, ICreatorFor<TResult>
  {
    new TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
  }

  //11
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, out TResult>
    : ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, ICreatorFor<TResult>
  {
    new TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);
  }

  //12
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, out TResult>
    : ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, ICreatorFor<TResult>
  {
    new TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);
  }

  //13
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, out TResult>
    : ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, ICreatorFor<TResult>
  {
    new TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);
  }

  //14
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, out TResult>
    : ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, ICreatorFor<TResult>
  {
    new TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);
  }

  //15
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, out TResult>
    : ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, ICreatorFor<TResult>
  {
    new TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);
  }

  //16
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, out TResult>
    : ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, ICreatorFor<TResult>
  {
    new TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);
  }
}