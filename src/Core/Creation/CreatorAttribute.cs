using System;


namespace WrathTools
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public sealed class CreatorAttribute : Attribute
  {
    //TODO: Analyzer to show warning that Creator methods should be called Create to allow overload rules to check for ambiguity
    //TODO: Analyzer for all Creator and NamedCreator methods to show warning if they are generic that they will be ignored.
  }
}