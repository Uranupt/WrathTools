using System;


namespace WrathTools
{
  public abstract class CreatorBase<TArg1, TArg2, TArg3, TArg4, TResult> : CreatorBase, ICreator<TArg1, TArg2, TArg3, TArg4,TResult>
  {

    private readonly Type[] _argumentTypes = new Type[] { typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) };

    public override Type CreatedType => typeof(TResult);
    public override Type[] ArgumentTypes => _argumentTypes;

    public abstract TResult Create(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);

    object ICreatorWithArgs<TArg1, TArg2, TArg3, TArg4>.Create(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
      => Create(arg1, arg2, arg3, arg4);

    TResult ICreatorFor<TResult>.Create(params object[] args)
    {
      if(!ArgumentCheck(args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return Create((TArg1)args[0], (TArg2)args[1], (TArg3)args[2], (TArg4)args[3]);
    }

    public override bool TryCreate(out object value, params object[] args)
    {
      if(ArgumentCheck(args))
      {
        value = Create((TArg1)args[0], (TArg2)args[1], (TArg3)args[2], (TArg4)args[3]);
        return true;
      }
      value = default;
      return false;
    }

  }
}