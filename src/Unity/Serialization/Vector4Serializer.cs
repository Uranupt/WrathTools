using UnityEngine;


namespace WrathTools.Unity.Serialization
{
  [BinarySerializer(typeof(Vector4), BinarySerialization.DefaultConverterName)]
  public static class Vector4Serializer
  {

    public static void Write(BinaryWriteContext context, Vector4 value)
    {
      context.Writer.Write(value.x);
      context.Writer.Write(value.y);
      context.Writer.Write(value.z);
      context.Writer.Write(value.w);
    }

    public static Vector3 Read(BinaryReadContext context)
    {
      return new Vector4(
        context.Reader.ReadSingle(),
        context.Reader.ReadSingle(),
        context.Reader.ReadSingle(),
        context.Reader.ReadSingle()
      );
    }

  }
}