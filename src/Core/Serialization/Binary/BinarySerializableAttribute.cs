using System;


namespace WrathTools
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
  public sealed class BinarySerializableAttribute : Attribute
  {

    public SerializationBehavior Behavior;

    public BinarySerializableAttribute(SerializationBehavior behavior)
    {
      Behavior = behavior;
    }

  }
}
