

namespace WrathTools
{
  public interface ICreator<in TArg, out TResult> : ICreatorWithArgs<TArg>, ICreatorFor<TResult>
  {

    new TResult Create(TArg arg);

  }
}
