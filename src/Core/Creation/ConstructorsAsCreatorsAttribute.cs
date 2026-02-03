using System;


namespace WrathTools
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
  public sealed class ConstructorsAsCreatorsAttribute : Attribute
  { 
    //TEST: ConstructorsAsCreatorsAttribute with all arities
    //TEST: ConstructorsAsCreatorsAttibute with generic versions
  }
}