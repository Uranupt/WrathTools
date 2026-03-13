using System;


namespace WrathTools.Unity.ResourceManagement
{
  public abstract class ResourceObject<T> : ResourceObject, IResourceObject<T> where T : class
  {

    public sealed override Type BuildType => typeof(T);

    public abstract T Build();

    public override sealed bool TryBuild<TResl>(out TResl resl, bool exactType = true) where TResl : class
    {

      if(exactType ? typeof(TResl) == BuildType : typeof(TResl).IsAssignableFrom(BuildType))
      {
        resl = Build() as TResl;
        return true;
      }
      resl = null;
      return false;
    }

    public override sealed TResl Build<TResl>(bool exactType = true) where TResl : class
    {
      if(!TryBuild(out TResl resl, exactType))
      {
        UnityDiagnostics.LogError(
          new InvalidCastException($"Cannot cast from Type {BuildType.Name} to Type {typeof(TResl).Name}"),
          stackTrace: new(true),
          id: ResourceDatabase.DiagnosticID + ".incorrect_resource_build_type"
        );
      }
      return resl;
    }

  }
}