using System;


namespace WrathTools
{
  public abstract class CreatorBase<TArg1, TArg2, TArg3, TResult> : CreatorBase, ICreator<TArg1, TArg2, TArg3, TResult>
  {

    private readonly ArgsSignature _signature = 
      new(new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) });

    public override Type CreatedType => typeof(TResult);
    public override ArgsSignature Signature => _signature;

    public abstract TResult Create(TArg1 arg1, TArg2 arg2, TArg3 arg3);

    object ICreatorWithArgs<TArg1, TArg2, TArg3>.Create(TArg1 arg1, TArg2 arg2, TArg3 arg3) 
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
      return Create((TArg1)args[0], (TArg2)args[1], (TArg3)args[2]);
    }

    public override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((TArg1)args[0], (TArg2)args[1], (TArg3)args[2]);
      return true;
    }

  }
}