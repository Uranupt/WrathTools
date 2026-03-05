using System;


namespace WrathTools
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false,Inherited = false)]
  public sealed class NamedCreatorAttribute : Attribute
  {
    public string Name;

    public NamedCreatorAttribute(string name)
    {
      Name = name;
    }

  }
}
