using UnityEngine;


namespace WrathTools.Unity.Serialization
{
  [BinarySerializer(typeof(Vector2), BinarySerialization.DefaultConverterName)]
  public static class Vector2Serializer
  {

    public static void Write(BinaryWriteContext context, Vector2 value)
    {
      context.Writer.Write(value.x);
      context.Writer.Write(value.y);
    }

    public static Vector2 Read(BinaryReadContext context)
    {
      return new Vector2(
        context.Reader.ReadSingle(),
        context.Reader.ReadSingle()
      );
    }

  }
}
