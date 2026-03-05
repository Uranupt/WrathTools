

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

    public static void Write(BinaryWriteContext context, ManualSelfSerializer instance)
    {
      context.WriteAs<string>(instance._privateUnmarked);
      context.WriteAs<string>(instance.PublicUnmarked);
    }

    public static ManualSelfSerializer Read(BinaryReadContext context)
    {
      ManualSelfSerializer instance = new();
      instance._privateUnmarked = context.ReadAs<string>();
      instance.PublicUnmarked = context.ReadAs<string>();
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