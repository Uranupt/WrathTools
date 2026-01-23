using System;


namespace WrathTools
{
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class BinarySerializerAttribute : Attribute
  {

    public string Name;
    public Type Target;

    public BinarySerializerAttribute(Type target, string name = null)
    {
      Name = name;
      Target = target;
    }

  }
}