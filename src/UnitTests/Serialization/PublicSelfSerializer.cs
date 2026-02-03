

namespace WrathTools.UnitTests
{
  [BinarySerializable(SerializationBehavior.PublicFields)]
  public sealed class PublicSelfSerializer
  {

    private string _privateUnmarked = "Default";
    [SerializeBinary] private string _privateMarked = "Default";
    public string PublicUnmarked = "Default";
    [SerializeBinary] public string PublicMarked = "Default";

    private PublicSelfSerializer()
    {

    }

    public PublicSelfSerializer(string setValue)
    {
      _privateUnmarked = setValue;
      _privateMarked = setValue;
      PublicUnmarked = setValue;
      PublicMarked = setValue;
    }

    [Creator]
    public static PublicSelfSerializer Create() => new();

    public bool CheckValues(string setValue)
    {
      return _privateUnmarked == "Default"
        && _privateMarked == setValue
        && PublicUnmarked == setValue
        && PublicMarked == setValue;
    }

  }
}