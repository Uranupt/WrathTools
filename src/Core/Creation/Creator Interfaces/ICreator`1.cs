

namespace WrathTools
{ 
  public interface ICreator<out TResult> : ICreatorWithoutArgs, ICreatorFor<TResult>
  {
    new TResult Create();
  }
}
