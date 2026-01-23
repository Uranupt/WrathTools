using System;


namespace WrathTools
{
  public abstract class CreatorBase<TArg, TResult> : CreatorBase, ICreator<TArg, TResult>
  {

    private readonly ArgsSignature _signature = new(new Type[] { typeof(TArg) });

    public override Type CreatedType => typeof(TResult);
    public override ArgsSignature Signature => _signature;

    public abstract TResult Create(TArg arg);

    object ICreatorWithArgs<TArg>.Create(TArg arg) => Create(arg);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((TArg)args[0]);
    }

    public override bool TryCreate(out object value, params object[] args)
    {
      if(!Signature.CanAccept(args))
      {
        value = default;
        return false;
      }
      value = Create((TArg)args[0]);
      return true;
    }

  }
}
