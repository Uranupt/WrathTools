

namespace WrathTools
{
  public interface ICreatorFor<out TResult> : ICreator
  {

    new TResult Create(params object[] args);

  }
}
