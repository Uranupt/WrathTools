using System;


namespace WrathTools
{

  //1
  public abstract class CreatorBase<TResult>
    : CreatorBase,
      ICreator<TResult>
  {
    private readonly ArgsSignature _signature =
      new(Array.Empty<Type>());
    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create();

    object ICreatorWithoutArgs
      .Create() 
      => Create();

    TResult ICreatorFor<TResult>.Create(params object[] args) 
      => Create();

    public sealed override bool TryCreate(out object value, params object[] args)
    {
      value = Create();
      return true;
    }
  }



  //2
  public abstract class CreatorBase<T, TResult>
    : CreatorBase,
      ICreator<T, TResult>
  {
    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(T) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(T arg);

    object ICreatorWithArgs<T>
      .Create(T arg) 
      => Create(arg);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((T)args[0]);
    }

    public sealed override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((T)args[0]);
      return true;
    }
  }



  //3
  public abstract class CreatorBase<T1, T2, TResult>
    : CreatorBase,
      ICreator<T1, T2, TResult>
  {
    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(T1), typeof(T2) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(T1 arg1, T2 arg2);

    object ICreatorWithArgs<T1, T2>
      .Create(T1 arg1, T2 arg2)
      => Create(arg1, arg2);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((T1)args[0], (T2)args[1]);
    }

    public sealed override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((T1)args[0], (T2)args[1]);
      return true;
    }
  }



  //4
  public abstract class CreatorBase<T1, T2, T3, TResult>
    : CreatorBase,
      ICreator<T1, T2, T3, TResult>
  {
    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(T1), typeof(T2), typeof(T3) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(T1 arg1, T2 arg2, T3 arg3);

    object ICreatorWithArgs<T1, T2, T3>
      .Create(T1 arg1, T2 arg2, T3 arg3)
      => Create(arg1, arg2, arg3);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((T1)args[0], (T2)args[1], (T3)args[2]);
    }

    public sealed override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((T1)args[0], (T2)args[1], (T3)args[2]);
      return true;
    }
  }



  //5
  public abstract class CreatorBase<T1, T2, T3, T4, TResult>
    : CreatorBase,
      ICreator<T1, T2, T3, T4, TResult>
  {
    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    object ICreatorWithArgs<T1, T2, T3, T4>
      .Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
      => Create(arg1, arg2, arg3, arg4);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]);
    }

    public sealed override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]);
      return true;
    }
  }



  //6
  public abstract class CreatorBase<T1, T2, T3, T4, T5, TResult>
    : CreatorBase,
      ICreator<T1, T2, T3, T4, T5, TResult>
  {
    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    object ICreatorWithArgs<T1, T2, T3, T4, T5>
      .Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
      => Create(arg1, arg2, arg3, arg4, arg5);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4]);
    }

    public sealed override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4]);
      return true;
    }
  }



  //7
  public abstract class CreatorBase<T1, T2, T3, T4, T5, T6, TResult>
    : CreatorBase,
      ICreator<T1, T2, T3, T4, T5, T6, TResult>
  {
    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    object ICreatorWithArgs<T1, T2, T3, T4, T5, T6>
      .Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
      => Create(arg1, arg2, arg3, arg4, arg5, arg6);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5]);
    }

    public override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5]);
      return true;
    }
  }



  //8
  public abstract class CreatorBase<T1, T2, T3, T4, T5, T6, T7, TResult>
    : CreatorBase,
      ICreator<T1, T2, T3, T4, T5, T6, T7, TResult>
  {
    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

    object ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7>.Create(
      T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
      => Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6]);
    }

    public override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6]);
      return true;
    }
  }



  //9
  public abstract class CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
    : CreatorBase,
      ICreator<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
  {
    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

    object ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8>
      .Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
      => Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7]);
    }

    public override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7]);
      return true;
    }
  }



  //10
  public abstract class CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
    : CreatorBase,
      ICreator<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
  {
    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);

    object ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9>
      .Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
      => Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7], (T9)args[8]);
    }

    public override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7], (T9)args[8]);
      return true;
    }
  }



  //11
  public abstract class CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
    : CreatorBase,
      ICreator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
  {
    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);

    object ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
      .Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
      => Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7], (T9)args[8], (T10)args[9]);
    }

    public override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7], (T9)args[8], (T10)args[9]);
      return true;
    }
  }



  //12
  public abstract class CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>
    : CreatorBase,
      ICreator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>
  {
    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);

    object ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
      .Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
      => Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7], (T9)args[8], (T10)args[9], (T11)args[10]);
    }

    public override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7], (T9)args[8], (T10)args[9], (T11)args[10]);
      return true;
    }
  }



  //13
  public abstract class CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>
    : CreatorBase,
      ICreator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>
  {
    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);

    object ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
      .Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
      => Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7], (T9)args[8], (T10)args[9], (T11)args[10], (T12)args[11]);
    }

    public override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7], (T9)args[8], (T10)args[9], (T11)args[10], (T12)args[11]);
      return true;
    }
  }



  //14
  public abstract class CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>
    : CreatorBase,
      ICreator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>
  {
    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);

    object ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
      .Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
      => Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7], (T9)args[8], (T10)args[9], (T11)args[10], (T12)args[11], (T13)args[12]);
    }

    public override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7], (T9)args[8], (T10)args[9], (T11)args[10], (T12)args[11], (T13)args[12]);
      return true;
    }
  }



  //14
  public abstract class CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
    : CreatorBase,
      ICreator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
  {
    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);

    object ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
      .Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
      => Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7], (T9)args[8], (T10)args[9], (T11)args[10], (T12)args[11], (T13)args[12], (T14)args[13]);
    }

    public override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7], (T9)args[8], (T10)args[9], (T11)args[10], (T12)args[11], (T13)args[12], (T14)args[13]);
      return true;
    }
  }



  //16
  public abstract class CreatorBase<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
    : CreatorBase,
      ICreator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
  {
    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);

    object ICreatorWithArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
      .Create(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
      => Create(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7], (T9)args[8], (T10)args[9], (T11)args[10], (T12)args[11], (T13)args[12], (T14)args[13], (T15)args[14]);
    }

    public override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6], (T8)args[7], (T9)args[8], (T10)args[9], (T11)args[10], (T12)args[11], (T13)args[12], (T14)args[13], (T15)args[14]);
      return true;
    }
  }
}