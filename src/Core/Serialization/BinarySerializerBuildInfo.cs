using System;


namespace WrathTools
{
  internal class BinarySerializerBuildInfo
  {

    public Type TargetedType;
    public Type DeclaringType;
    public string Name;
    public SerializationBehavior Behavior;

    public BinarySerializerBuildInfo(Type type, BinarySerializableAttribute attribute)
    {
      DeclaringType = type;
      TargetedType = type;
      Name = BinarySerialization.DefaultConverterName;
      Behavior = attribute.Behavior;
    }

    public BinarySerializerBuildInfo(Type type, BinarySerializerAttribute attribute)
    {
      DeclaringType = type;
      TargetedType = attribute.Target;
      Behavior = SerializationBehavior.Manual;
      Name = attribute.Name;
    }

  }
}