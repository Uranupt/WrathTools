

namespace WrathTools
{
  public interface ICreator<in TArg, out TResult> : ICreatorFrom<TArg>
  {

    TResult Create(TArg arg);

  }
}
