

namespace WrathTools.Unity.ResourceManagement
{
  public sealed class BuilderHandle<T> : ResourceHandle, IBuilder<T> where T : class
  {

    public BuilderHandle(int id, bool exactType = true) : base(id, typeof(T), exactType)
    {

    }

    public T Build() => Resource.Build<T>();

  }
}
