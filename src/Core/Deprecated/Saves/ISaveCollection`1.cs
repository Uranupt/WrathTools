using System;
using System.Collections.Generic;


namespace WrathTools.Deprecated
{
  [Obsolete("SaveBridge has been deprecated in favor of BinarySerialization")]
  public interface ISaveCollection<TSave>
  {
    IReadOnlyCollection<TSave> Collection { get; }
    void Add(TSave save);
  }
}