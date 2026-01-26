using System;


namespace WrathTools.Deprecated
{
  [AttributeUsage(AttributeTargets.Field)]
  [Obsolete("SaveBridge has been deprecated in favor of BinarySerialization")]
  public sealed class AutoSaveFieldAttribute : Attribute
  {

    public int Order { get; private set; }

    public AutoSaveFieldAttribute(int order)
    {
      Order = order;
    }

  }
}
