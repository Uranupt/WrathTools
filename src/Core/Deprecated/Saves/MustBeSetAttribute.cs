using System;


namespace WrathTools.Deprecated
{
  [AttributeUsage(AttributeTargets.Field)]
  [Obsolete("SaveBridge has been deprecated in favor of BinarySerialization")]
  public sealed class MustBeSetAttribute : Attribute 
  {

    public MustBeSetAttribute()
    {

    }

  }
}