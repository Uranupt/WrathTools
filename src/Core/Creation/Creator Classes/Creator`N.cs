

using System;

namespace WrathTools
{

  //1
  public sealed class Creator<TResult>
    : CreatorBase<TResult>
  {
    private readonly string _name;
    private readonly Func<TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create()
      => _create.Invoke();
  }

  //2
  public sealed class Creator<T, TResult>
    : CreatorBase<T, TResult>
  {
    private readonly string _name;
    private readonly Func<T, TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<T, TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(T arg)
      => _create.Invoke(arg);
  }

  //3
  public sealed class Creator<T1, T2, TResult>
    : CreatorBase<T1, T2, TResult>
  {
    private readonly string _name;
    private readonly Func<T1, T2, TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<T1, T2, TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(T1 arg1, T2 arg2)
      => _create.Invoke(arg1, arg2);
  }

  //4
  public sealed class Creator<T1, T2, T3, TResult>
    : CreatorBase<T1, T2, T3, TResult>
  {
    private readonly string _name;
    private readonly Func<T1, T2, T3, TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<T1, T2, T3, TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(T1 arg1, T2 arg2, T3 arg3)
      => _create.Invoke(arg1, arg2, arg3);
  }

  //5
  public sealed class Creator<T1, T2, T3, T4, TResult>
    : CreatorBase<T1, T2, T3, T4, TResult>
  {
    private readonly string _name;
    private readonly Func<T1, T2, T3, T4, TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<T1, T2, T3, T4, TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
      => _create.Invoke(arg1, arg2, arg3, arg4);
  }

  //6
  public sealed class Creator<T1, T2, T3, T4, T5, TResult>
    : CreatorBase<T1, T2, T3, T4, T5, TResult>
  {
    private readonly string _name;
    private readonly Func<T1, T2, T3, T4, T5, TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<T1, T2, T3, T4, T5, TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
      => _create.Invoke(arg1, arg2, arg3, arg4, arg5);
  }

  //7
  public sealed class Creator<T1, T2, T3, T4, T5, T6, TResult>
    : CreatorBase<T1, T2, T3, T4, T5, T6, TResult>
  {
    private readonly string _name;
    private readonly Func<T1, T2, T3, T4, T5, T6, TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<T1, T2, T3, T4, T5, T6, TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
      => _create.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
  }

  //8
  public sealed class Creator<T1, T2, T3, T4, T5, T6, T7, TResult>
    : CreatorBase<T1, T2, T3, T4, T5, T6, T7, TResult>
  {
    private readonly string _name;
    private readonly Func<T1, T2, T3, T4, T5, T6, T7, TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<T1, T2, T3, T4, T5, T6, T7, TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
      => _create.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
  }

  //9
  public sealed class Creator<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
    : CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
  {
    private readonly string _name;
    private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
      => _create.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
  }

  //10
  public sealed class Creator<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
    : CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
  {
    private readonly string _name;
    private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
      => _create.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
  }

  //11
  public sealed class Creator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
    : CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
  {
    private readonly string _name;
    private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
      => _create.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
  }

  //12
  public sealed class Creator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>
    : CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>
  {
    private readonly string _name;
    private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
      => _create.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
  }

  //13
  public sealed class Creator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>
    : CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>
  {
    private readonly string _name;
    private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
      => _create.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
  }

  //14
  public sealed class Creator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>
    : CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>
  {
    private readonly string _name;
    private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
      => _create.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
  }

  //15
  public sealed class Creator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
    : CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
  {
    private readonly string _name;
    private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
      => _create.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
  }

  //16
  public sealed class Creator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
    : CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
  {
    private readonly string _name;
    private readonly Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> _create;
    public override string Name => _name;

    public Creator(
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> create,
      string name)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
      => _create.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
  }
}