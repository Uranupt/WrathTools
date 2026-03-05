using UnityEngine;


namespace WrathTools.Unity.Serialization
{
  [BinarySerializer(typeof(Quaternion), "packed")]
  public static class PackedQuaternionSerializer
  {

    public static void Write(BinaryWriteContext context, Quaternion value)
    {
      context.Writer.Write(UnityTools.PackQuaternion(value));
    }

    public static Quaternion Read(BinaryReadContext context)
    {
      return UnityTools.UnpackQuaternion(context.Reader.ReadUInt32());
    }

  }
}
