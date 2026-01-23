using System;


namespace WrathTools
{
  public sealed class Creator<TArg1, TArg2, TArg3, TResult> : CreatorBase<TArg1, TArg2, TArg3, TResult>
  {

    private readonly string _name;
    private readonly Func<TArg1, TArg2, TArg3, TResult> _create;
    public override string Name => _name;

    public Creator(Func<TArg1, TArg2, TArg3, TResult> create, string name = Creators.DefaultCreatorName)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(TArg1 arg1, TArg2 arg2, TArg3 arg3) => _create.Invoke(arg1, arg2, arg3);

  }
}