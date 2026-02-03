using System;


namespace WrathTools
{
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class DoNotSerializeAttribute : Attribute
  {

  }
}