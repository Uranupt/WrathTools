

using System;

namespace WrathTools.Deprecated
{
  [Obsolete("SaveBridge has been deprecated in favor of BinarySerialization")]
  public interface IAutoSaveProvider<TSelf> : IAutoSaveProvider, ISaveProvider<AutoSaveObject<TSelf>>
    where TSelf : class, IAutoSaveProvider<TSelf>
  {

  }
}
