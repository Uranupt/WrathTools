

namespace WrathTools.UnitTests
{
  public static class NamedCreators<T>
  {

    [NamedCreator("unit_test")]
    public static NoCreator<T> NoCreatorN0() => new NoCreator<T>();

    [NamedCreator("unit_test")]
    public static NoCreator<T> NoCreatorN1(int a1) => new NoCreator<T>();

  }
}