

namespace WrathTools.Unity.ResourceManagement
{
  public interface IResourceObject<out T> : IResourceObject where T : class
  {

    T Build();

  }
}