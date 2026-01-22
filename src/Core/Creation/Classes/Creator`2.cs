using System;


namespace WrathTools
{ 
  public sealed class Creator<TArg, TResult> : CreatorBase<TArg, TResult>
  {

    private readonly Func<TArg, TResult> _create;

    public Creator(Func<TArg, TResult> create)
    {
      _create = create;
    }

    public override TResult Create(TArg arg) => _create.Invoke(arg);

  }
}