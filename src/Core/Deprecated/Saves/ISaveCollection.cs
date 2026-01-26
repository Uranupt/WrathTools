using System;
using System.Collections.Generic;


namespace WrathTools.Deprecated
{
  [Obsolete("SaveBridge has been deprecated in favor of BinarySerialization")]
  public interface ISaveCollection
  { 

    IReadOnlyCollection<SaveObject> Saves { get; }

  }
}
