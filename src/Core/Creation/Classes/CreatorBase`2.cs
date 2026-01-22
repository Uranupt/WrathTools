using System;


namespace WrathTools
{
  public abstract class CreatorBase<TArg, TResult> : CreatorBase, ICreator<TArg, TResult>
  {

    private readonly Type[] _argumentTypes = new Type[] { typeof(TArg) };

    public override Type CreatedType => typeof(TResult);
    public override Type[] ArgumentTypes => _argumentTypes;

    public abstract TResult Create(TArg arg);

    object ICreatorWithArgs<TArg>.Create(TArg arg) => Create(arg);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!ArgumentCheck(args))
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
      if(ArgumentCheck(args))
      {
        value = Create((TArg)args[0]);
        return true;
      }
      value = default;
      return false;
    }

  }
}
