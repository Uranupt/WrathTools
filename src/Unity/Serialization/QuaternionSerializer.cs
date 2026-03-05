using UnityEngine;


namespace WrathTools.Unity.Serialization
{
  [BinarySerializer(typeof(Quaternion), BinarySerialization.DefaultConverterName)]
  public static class QuaternionSerializer
  {

    public static void Write(BinaryWriteContext context, Quaternion value)
    {
      context.Writer.Write(value.x);
      context.Writer.Write(value.y);
      context.Writer.Write(value.z);
      context.Writer.Write(value.w);
    }

    public static Quaternion Read(BinaryReadContext context)
    {
      return new Quaternion(
        context.Reader.ReadSingle(),
        context.Reader.ReadSingle(),
        context.Reader.ReadSingle(),
        context.Reader.ReadSingle()
      );
    }

  }
}