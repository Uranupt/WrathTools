

namespace WrathTools.UnitTests
{
  public static class NamedCreators
  {

    [NamedCreator("unit_test")]
    public static NoCreator NoCreatorN0() => new NoCreator();

    [NamedCreator("unit_test")]
    public static NoCreator NoCreatorN1(int a1) => new NoCreator();

    [NamedCreator("unit_test_alt")]
    public static NoCreator NoCreatorN0Alt() => new NoCreator();

    [NamedCreator("unit_test_alt")]
    public static NoCreator NoCreatorN1Alt(int a1) => new NoCreator();

    [NamedCreator("unit_test")]
    public static SelfCreator SelfCreatorN0() => new SelfCreator();

  }
}