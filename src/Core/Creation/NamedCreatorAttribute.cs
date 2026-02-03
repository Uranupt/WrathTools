using System;


namespace WrathTools
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false,Inherited = false)]
  public sealed class NamedCreatorAttribute : Attribute
  {
    //TEST: NamedCreatorAttribute w/ all arities
    //TEST: ConstructorsAsCreatorsAttibute with generic versions
    public string Name;

    public NamedCreatorAttribute(string name)
    {
      Name = name;
    }

  }
}
