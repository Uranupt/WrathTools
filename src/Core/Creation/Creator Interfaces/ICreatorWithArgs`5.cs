

namespace WrathTools
{
  public interface ICreatorWithArgs<in TArg1, in TArg2, in TArg3, in TArg4, in TArg5> : ICreator
  {

    object Create(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);

  }
}