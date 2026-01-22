

namespace WrathTools
{
  public interface ICreatorWithArgs<in TArg> : ICreator
  {

    object Create(TArg arg);

  }
}