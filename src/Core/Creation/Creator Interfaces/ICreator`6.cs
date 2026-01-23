

namespace WrathTools
{
  public interface ICreator<in TArg1, in TArg2, in TArg3, in TArg4, in TArg5, out TResult> :
    ICreatorWithArgs<TArg1, TArg2, TArg3, TArg4, TArg5>, ICreatorFor<TResult>
  {

    new TResult Create(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5);

  }
}
