using System;


namespace WrathTools
{
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class AutoSaveFieldAttribute : Attribute
  {

    public int Order { get; private set; }

    public AutoSaveFieldAttribute(int order)
    {
      Order = order;
    }

  }
}
