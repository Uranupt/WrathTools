using System;


namespace WrathTools.Unity.ResourceManagement
{
  public interface IResourceObject
  {

    int ID { get; }
    Type BuildType { get; }
    bool TryBuild<T>(out T resl, bool exactType = true) where T : class;
    T Build<T>(bool exactType = true) where T : class;

  }
}