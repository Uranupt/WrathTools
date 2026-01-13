using System;
using System.Collections.Generic;
using UnityEngine;


namespace WrathTools.Unity.ResourceManagement
{
  [Serializable]
  public abstract class ResourcePeek
  {

    [field: SerializeField] public string Name { get; internal set; }
    [field: SerializeField] public int ID { get; internal set; }
    public abstract Type ResourceType { get; }

    public abstract IEnumerator<string> AllNames();
    public abstract IEnumerator<dynamic> AllItems();
    public abstract bool TryGetItem<T>(string name, out T resl, bool exactType = true);

    public T GetItem<T>(string name, bool exactType = true)
    {
      if(TryGetItem(name, out T item, exactType))
      {
        return item;
      }
      throw new KeyNotFoundException($"No field matching the name or alias {name} was found.");
    }

  }
}
