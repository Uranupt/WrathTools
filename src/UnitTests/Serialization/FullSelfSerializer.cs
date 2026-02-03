

namespace WrathTools.UnitTests
{
  [BinarySerializable(SerializationBehavior.AllFields)]
  public sealed class FullSelfSerializer
  {

    private string _privateUnmarked = "Default";
    [SerializeBinary] private string _privateMarked = "Default";
    public string PublicUnmarked = "Default";
    [SerializeBinary] public string PublicMarked = "Default";

    private FullSelfSerializer()
    {

    }

    public FullSelfSerializer(string setValue)
    {
      _privateUnmarked = setValue;
      _privateMarked = setValue;
      PublicUnmarked = setValue;
      PublicMarked = setValue;
    }

    [Creator]
    public static FullSelfSerializer Create() => new();

    public bool CheckValues(string setValue)
    {
      return _privateUnmarked == setValue
        && _privateMarked == setValue
        && PublicUnmarked == setValue
        && PublicMarked == setValue;
    }

  }
}