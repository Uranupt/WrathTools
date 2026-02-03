

namespace WrathTools.UnitTests
{
  [BinarySerializable(SerializationBehavior.Manual)]
  public sealed class ManualSelfSerializer
  {

    private string _privateUnmarked = "Default";
    [SerializeBinary] private string _privateMarked = "Default";
    public string PublicUnmarked = "Default";
    [SerializeBinary] public string PublicMarked = "Default";

    private ManualSelfSerializer()
    {

    }

    public ManualSelfSerializer(string setValue)
    {
      _privateUnmarked = setValue;
      PublicUnmarked = setValue;
    }

    public static void Write(BinaryWriter writer, ManualSelfSerializer instance)
    {
      writer.Write(instance._privateUnmarked);
      writer.Write(instance.PublicUnmarked);
    }

    public static ManualSelfSerializer Read(BinaryReader reader)
    {
      ManualSelfSerializer instance = new();
      instance._privateUnmarked = reader.ReadString();
      instance.PublicUnmarked = reader.ReadString();
      return instance;
    }

    public bool CheckValues(string setValue)
    {
      return _privateUnmarked == setValue
        && _privateMarked == "Default"
        && PublicUnmarked == setValue
        && PublicMarked == "Default";
    }

  }
}