

namespace WrathTools
{
  public interface IScopeProvider< out T> : IScopeProvider where T : IScope
  {

    new T Enter();

  }
}
