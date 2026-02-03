

namespace WrathTools
{

  //1
  public interface ICreator<out TResult> 
    : ICreatorFor<TResult>
  {
    TResult Create();
  }

  //2
  public interface ICreator<in T, out TResult> 
    : ICreatorFor<TResult>
  {
    TResult Create(T arg);
  }

  //3
  public interface ICreator<in T1, in T2, out TResult> 
    : ICreatorFor<TResult>
  {
    TResult Create(T1 arg1, T2 arg2);
  }

  //4
  public interface ICreator<in T1, in T2, in T3, out TResult> 
    : ICreatorFor<TResult>
  {
    TResult Create(T1 arg1, T2 arg2, T3 arg3);
  }

  //5
  public interface ICreator<in T1, in T2, in T3, in T4, out TResult> 
    : ICreatorFor<TResult>
  {
    TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
  }

  //6
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, out TResult> 
    : ICreatorFor<TResult>
  {
    TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
  }

  //7
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, out TResult> 
    : ICreatorFor<TResult>
  {
    TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
  }

  //8
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out TResult> 
    : ICreatorFor<TResult>
  {
    TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
  }

  //9
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out TResult>
    : ICreatorFor<TResult>
  {
    TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
  }

  //10
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, out TResult>
    : ICreatorFor<TResult>
  {
    TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
  }

  //11
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, out TResult>
    : ICreatorFor<TResult>
  {
    TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);
  }

  //12
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, out TResult>
    : ICreatorFor<TResult>
  {
    TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);
  }

  //13
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, out TResult>
    : ICreatorFor<TResult>
  {
    TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);
  }

  //14
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, out TResult>
    : ICreatorFor<TResult>
  {
    TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);
  }

  //15
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, out TResult>
    : ICreatorFor<TResult>
  {
    TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);
  }

  //16
  public interface ICreator<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, out TResult>
    : ICreatorFor<TResult>
  {
    TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);
  }
}