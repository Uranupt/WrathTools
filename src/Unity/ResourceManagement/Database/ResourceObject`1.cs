using System;


namespace WrathTools.Unity.ResourceManagement
{
  public abstract class ResourceObject<T> : ResourceObject, IBuilder<T> where T : class
  {

    public sealed override Type BuildType => typeof(T);

    public abstract T Build();

    public override sealed bool TryBuild<TResl>(out TResl resl, bool exactType = true) where TResl : class
    {
      if(typeof(TResl).TypeMatch(BuildType, exactType))
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
        DiagnosticContext error = new UnityErrorContext(
          new InvalidCastException($"Cannot cast from Type {BuildType.Name} to Type {typeof(TResl).Name}"),
          stackTrace: new(true)
        );
        Diagnostics.Log(error);
      }
      return resl;
    }

  }
}