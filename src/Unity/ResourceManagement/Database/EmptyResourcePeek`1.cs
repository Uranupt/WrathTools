using System;
using System.Collections.Generic;
using System.Linq;


namespace WrathTools.Unity.ResourceManagement
{
  [Serializable]
  public class EmptyResourcePeek<T> : ResourcePeek
  {

    public override Type ResourceType => typeof(T);

    public override IEnumerator<string> AllNames() => Enumerable.Empty<string>().GetEnumerator();
    public override IEnumerator<dynamic> AllItems() => Enumerable.Empty<dynamic>().GetEnumerator();

    public override bool TryGetItem<TResl>(string name, out TResl resl, bool exactType = true)
    {
      resl = default;
      return false;
    }

  }
}
