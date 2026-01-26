

using System;

namespace WrathTools.Deprecated
{
  [Obsolete("SaveBridge has been deprecated in favor of BinarySerialization")]
  public interface ISaveProvider<TSave> where TSave : SaveObject
  {

    TSave BuildSave();

  }
}
