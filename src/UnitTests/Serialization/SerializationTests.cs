

namespace WrathTools.UnitTests
{
  public class SerializationTests
  {

    [Theory]
    [InlineData("test1")]
    //[InlineData("test2")]
    public void MarkedSelfSerializerTest(string setValue)
    {
      MarkedSelfSerializer instance = new(setValue);
      BinaryConverter<MarkedSelfSerializer> converter = (BinaryConverter<MarkedSelfSerializer>)typeof(MarkedSelfSerializer).GetBinaryConverter();
      MemoryStream stream = new();
      using(BinaryWriter writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true))
      {
        converter.Write(writer, instance);
      }
      stream.Position = 0;
      BinaryReader reader = new BinaryReader(stream);
      MarkedSelfSerializer instance2 = converter.Read(reader);
      Assert.True(instance2.CheckValues(setValue));
    }

    [Theory]
    [InlineData("test1")]
    [InlineData("test2")]
    public void PublicSelfSerializerTest(string setValue)
    {

    }

    [Theory]
    [InlineData("test1")]
    [InlineData("test2")]
    public void FullSelfSerializerTest(string setValue)
    {

    }

    [Theory]
    [InlineData("test1")]
    [InlineData("test2")]
    public void ManualSelfSerializerTest(string setValue)
    {

    }

  }
}