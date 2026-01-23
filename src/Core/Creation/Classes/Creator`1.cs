using System;


namespace WrathTools
{
  public sealed class Creator<TResult> : CreatorBase<TResult>
  {

    private readonly string _name;
    private readonly Func<TResult> _create;
    public override string Name => _name;

    public Creator(Func<TResult> create, string name = Creators.DefaultCreatorName)
    {
      _create = create;
      _name = name;
    }

    public override TResult Create() => _create.Invoke();

  }
}