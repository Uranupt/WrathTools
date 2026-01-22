

namespace WrathTools
{
  public interface ICreatorWithArgs<in TArg1, in TArg2> : ICreator
  {

    object Create(TArg1 arg1, TArg2 arg2);

  }
}
