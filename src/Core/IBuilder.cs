using System;


namespace WrathTools
{
  public interface IBuilder
  {
    Type BuildType { get; }
    bool TryBuild<T>(out T resl, bool exactType = true) where T : class;
    T Build<T>(bool exactType = true) where T : class;
  }

  public static class BuilderUtility
  {

    public static bool TypeMatch(this Type type, Type other, bool exactMatch = true)
    {
      return exactMatch ? type == other : type.IsAssignableFrom(other);
    }

  }
}
