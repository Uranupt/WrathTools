using System;


namespace WrathTools
{
  public abstract class CreatorBase<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>
    : CreatorBase, ICreator<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>
  {

    private readonly ArgsSignature _signature =
      new(new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6) });

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6);

    object ICreatorWithArgs<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>.Create(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5,
      TArg6 arg6)
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
      return Create((TArg1)args[0], (TArg2)args[1], (TArg3)args[2], (TArg4)args[3], (TArg5)args[4], (TArg6)args[5]);
    }

    public override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((TArg1)args[0], (TArg2)args[1], (TArg3)args[2], (TArg4)args[3], (TArg5)args[4], (TArg6)args[5]);
      return true;
    }

  }
}