using System;


namespace WrathTools
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
  public sealed class BinarySerializableAttribute : Attribute
  {

    public bool SerializePublic;

    public BinarySerializableAttribute(bool serializePublic)
    {
      SerializePublic = serializePublic;
    }

  }
}
