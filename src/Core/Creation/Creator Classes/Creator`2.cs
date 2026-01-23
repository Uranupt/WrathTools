using System;


namespace WrathTools
{ 
  public sealed class Creator<TArg, TResult> : CreatorBase<TArg, TResult>
  {

    private readonly string _name;
    private readonly Func<TArg, TResult> _create;
    public override string Name => _name;

    public Creator(Func<TArg, TResult> create, string name = Creators.DefaultCreatorName)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create(TArg arg) => _create.Invoke(arg);

  }
}