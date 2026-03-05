using System;


namespace WrathTools
{
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
  public sealed class SerializeBinaryAttribute : Attribute
  {

    public readonly string SerializerName;

    public SerializeBinaryAttribute(string serializerName = null)
    {
      SerializerName = serializerName;
    }

  }
}