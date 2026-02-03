

namespace WrathTools.UnitTests
{
  [BinarySerializable(SerializationBehavior.MarkedFields)]
  public sealed class MarkedSelfSerializer
  {

    private string _privateUnmarked = "Default";
    [SerializeBinary] private string _privateMarked = "Default";
    public string PublicUnmarked = "Default";
    [SerializeBinary] public string PublicMarked = "Default";

    private MarkedSelfSerializer()
    {

    }

    public MarkedSelfSerializer(string setValue)
    {
      _privateUnmarked = setValue;
      _privateMarked = setValue;
      PublicUnmarked = setValue;
      PublicMarked = setValue;
    }

    [Creator]
    public static MarkedSelfSerializer Create() => new();

    public bool CheckValues(string setValue)
    {
      return _privateUnmarked == "Default"
        && _privateMarked == setValue
        && PublicUnmarked == "Default"
        && PublicMarked == setValue;
    }


  }
}
