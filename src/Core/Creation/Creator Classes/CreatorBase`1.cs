using System;


namespace WrathTools
{
  public abstract class CreatorBase<TResult> : CreatorBase, ICreator<TResult>
  {

    private readonly ArgsSignature _signature = new(new Type[0]);

    public sealed override Type CreatedType => typeof(TResult);
    public sealed override ArgsSignature Signature => _signature;

    public abstract TResult Create();

    object ICreatorWithoutArgs.Create() => Create();

    TResult ICreatorFor<TResult>.Create(params object[] args) => Create();

    public sealed override bool TryCreate(out object value, params object[] args)
    {
      value = Create();
      return true;
    }

  }
}
