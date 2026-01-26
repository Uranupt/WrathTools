

namespace WrathTools
{
  //1
  public interface ICreatorWithArgs<in T>
    : ICreator
  {
    object Create(T arg);
  }

  //2
  public interface ICreatorWithArgs<in T1, in T2>
    : ICreator
  {
    object Create(T1 arg1, T2 arg2);
  }

  //3
  public interface ICreatorWithArgs<in T1, in T2, in T3>
    : ICreator
  {
    object Create(T1 arg1, T2 arg2, T3 arg3);
  }

  //4
  public interface ICreatorWithArgs<in T1, in T2, in T3, in T4>
    : ICreator
  {
    object Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
  }

  //5
  public interface ICreatorWithArgs<in T1, in T2, in T3, in T4, in T5>
    : ICreator
  {
    object Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
  }

  //6
  public interface ICreatorWithArgs<in T1, in T2, in T3, in T4, in T5, in T6>
    : ICreator
  {
    object Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
  }

  //7
  public interface ICreatorWithArgs<in T1, in T2, in T3, in T4, in T5, in T6, in T7>
    : ICreator
  {
    object Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
  }

  //8
  public interface ICreatorWithArgs<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>
    : ICreator
  {
    object Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
  }

  //9
  public interface ICreatorWithArgs<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9>
    : ICreator
  {
    object Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
  }

  //10
  public interface ICreatorWithArgs<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10>
    : ICreator
  {
    object Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);
  }

  //11
  public interface ICreatorWithArgs<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11>
    : ICreator
  {
    object Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);
  }

  //12
  public interface ICreatorWithArgs<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12>
    : ICreator
  {
    object Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);
  }

  //13
  public interface ICreatorWithArgs<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13>
    : ICreator
  {
    object Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);
  }

  //14
  public interface ICreatorWithArgs<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14>
    : ICreator
  {
    object Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);
  }

  //15
  public interface ICreatorWithArgs<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15>
    : ICreator
  {
    object Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);
  }
}
