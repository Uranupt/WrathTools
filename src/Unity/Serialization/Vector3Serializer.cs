using UnityEngine;


namespace WrathTools.Unity.Serialization
{
  [BinarySerializer(typeof(Vector3), BinarySerialization.DefaultConverterName)]
  public static class Vector3Serializer
  {

    public static void Write(BinaryWriteContext context, Vector3 value)
    {
      context.Writer.Write(value.x);
      context.Writer.Write(value.y);
      context.Writer.Write(value.z);
    }

    public static Vector3 Read(BinaryReadContext context)
    {
      return new Vector3(
        context.Reader.ReadSingle(),
        context.Reader.ReadSingle(),
        context.Reader.ReadSingle()
      );
    }

  }
}