using System;


namespace WrathTools
{
  public sealed class Creator<TResult> : CreatorBase<TResult>
  {

    private readonly Func<TResult> _create;

    public Creator(Func<TResult> create)
    {
      _create = create;
    }

    public override TResult Create() => _create.Invoke();

  }
}