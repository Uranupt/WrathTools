using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace WrathTools.Unity.ResourceManagement
{
  public static class ResourceCache
  {

    private static readonly Dictionary<int, ResourceObject> _cache = new();
    private static readonly Dictionary<int, int> _handleCounts = new();
    internal static event Action Purged;

    public static async Task Purge()
    {
      Purged?.Invoke();
      _handleCounts.Clear();
      _cache.Clear();
      await Resources.UnloadUnusedAssets();
    }

    internal static bool TryGetResource(int id, out ResourceObject resl)
    {
      if(!_cache.TryGetValue(id, out resl))
      {
        LogHandle(id);
        if(!_cache.TryGetValue(id, out resl)){ return false; }
      }
      return true;
    }

    internal static bool TryGetResource(int id, Type type, out ResourceObject resl, bool exactType = true)
    {
      if(!TryGetResource(id, out resl)){ return false; }
      bool typeMatch = type.TypeMatch(resl.GetType(), exactType);
      if(!typeMatch)
      {
        resl = null;
      }
      return typeMatch;
    }

    internal static bool TryGetResource<T>(int id, out T resl, bool exactType = true) where T : ResourceObject
    {
      bool success = TryGetResource(id, typeof(T), out ResourceObject value, exactType);
      resl = success ? value as T : null;
      return success;
    }

    internal static void LogHandle(int id)
    {
      if(_handleCounts.ContainsKey(id))
      {
        _handleCounts[id]++;
      }
      else
      {
        if(!_cache.TryGetValue(id, out ResourceObject value) || value == null)
        {
          if(!ResourceID.TryGetResourcePath(id, out string path))
          {
            Diagnostics.Log(new UnityErrorContext(new Exception($"Failed to find path for resource with ID: {id.ToIDString(true)}")));
            return;
          }
          value = Resources.Load<ResourceObject>(path);
          if(value == null)
          {
            Diagnostics.Log(new UnityErrorContext(new Exception($"Resources.Load returned null at path: {path}")));
            return;
          }
          _cache[id] = value;
        }
        _handleCounts[id] = 1;
      }
    }

    internal static void ReleaseHandle(int id)
    {
      if(!_handleCounts.ContainsKey(id)) { return; }
      if(--_handleCounts[id] <= 0)
      {
        _handleCounts.Remove(id);
        Resources.UnloadAsset(_cache[id]);
        _cache.Remove(id);
      }
    }


  }
}