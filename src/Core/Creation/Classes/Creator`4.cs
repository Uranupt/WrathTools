using System;


namespace WrathTools
{
  public sealed class Creator<TArg1, TArg2, TArg3, TResult> : CreatorBase<TArg1, TArg2, TArg3, TResult>
  {

    private readonly Func<TArg1, TArg2, TArg3, TResult> _create;

    public Creator(Func<TArg1, TArg2, TArg3, TResult> create)
    {
      _create = create;
    }

    public override TResult Create(TArg1 arg1, TArg2 arg2, TArg3 arg3) => _create.Invoke(arg1, arg2, arg3);

  }
}