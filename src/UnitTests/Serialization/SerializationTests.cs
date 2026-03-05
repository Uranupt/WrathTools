

namespace WrathTools.UnitTests
{
  public class SerializationTests
  {

    [Theory]
    [InlineData("test1")]
    [InlineData("test2")]
    public void MarkedSelfSerializerTest(string setValue)
    {
      MarkedSelfSerializer writeInstance = new(setValue);
      BinaryConverter<MarkedSelfSerializer> converter = (BinaryConverter<MarkedSelfSerializer>)typeof(MarkedSelfSerializer).GetBinaryConverter();
      MemoryStream stream = new();
      using(BinaryWriter writer = new(stream, System.Text.Encoding.UTF8, true))
      {
        converter.Write(writer, writeInstance);
      }
      stream.Position = 0;
      BinaryReader reader = new(stream);
      MarkedSelfSerializer readInstance = converter.Read(reader);
      Assert.True(readInstance.CheckValues(setValue));
    }

    [Theory]
    [InlineData("test1")]
    [InlineData("test2")]
    public void PublicSelfSerializerTest(string setValue)
    {
      PublicSelfSerializer writeInstance = new(setValue);
      BinaryConverter<PublicSelfSerializer> converter = BinarySerialization.GetConverter<PublicSelfSerializer>();
      MemoryStream stream = new();
      using(BinaryWriter writer = new(stream, System.Text.Encoding.UTF8, true))
      {
        converter.Write(writer, writeInstance);
      }
      stream.Position = 0;
      BinaryReader reader = new(stream);
      PublicSelfSerializer readInstance = converter.Read(reader);
      Assert.True(readInstance.CheckValues(setValue));
    }

    [Theory]
    [InlineData("test1")]
    [InlineData("test2")]
    public void FullSelfSerializerTest(string setValue)
    {
      FullSelfSerializer writeInstance = new(setValue);
      BinaryConverter<FullSelfSerializer> converter = BinarySerialization.GetConverter<FullSelfSerializer>();
      MemoryStream stream = new();
      using(BinaryWriter writer = new(stream, System.Text.Encoding.UTF8, true))
      {
        converter.Write(writer, writeInstance);
      }
      stream.Position = 0;
      BinaryReader reader = new(stream);
      FullSelfSerializer readInstance = converter.Read(reader);
      Assert.True(readInstance.CheckValues(setValue));
    }

    [Theory]
    [InlineData("test1")]
    [InlineData("test2")]
    public void ManualSelfSerializerTest(string setValue)
    {
      ManualSelfSerializer writeInstance = new(setValue);
      BinaryConverter<ManualSelfSerializer> converter = BinarySerialization.GetConverter<ManualSelfSerializer>();
      MemoryStream stream = new();
      using(BinaryWriter writer = new(stream, System.Text.Encoding.UTF8, true))
      {
        converter.Write(writer, writeInstance);
      }
      stream.Position = 0;
      BinaryReader reader = new(stream);
      ManualSelfSerializer readInstance = converter.Read(reader);
      Assert.True(readInstance.CheckValues(setValue));
    }

    [Fact]
    public void ManualGraphSerializerTest()
    {
      ManualGraphSerializer writeInstance = new();
      BinaryConverter<ManualGraphSerializer> converter = BinarySerialization.GetConverter<ManualGraphSerializer>();
      MemoryStream stream = new();
      using(BinaryWriter writer = new(stream, System.Text.Encoding.UTF8, true))
      {
        converter.Write(writer, writeInstance);
      }
      stream.Position = 0;
      BinaryReader reader = new(stream);
      ManualGraphSerializer readInstance = converter.Read(reader);
      Assert.True(readInstance.TestReferences());
    }

  }
}