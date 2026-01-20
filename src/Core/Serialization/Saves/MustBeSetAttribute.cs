using System;


namespace WrathTools
{
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class MustBeSetAttribute : Attribute 
  {

    public MustBeSetAttribute()
    {

    }

  }
}