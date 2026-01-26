using System;


namespace WrathTools
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
  public sealed class ConstructorsAsCreatorsAttribute : Attribute
  { 
  }
}