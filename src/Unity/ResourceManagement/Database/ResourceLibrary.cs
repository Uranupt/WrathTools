using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;


namespace WrathTools.Unity.ResourceManagement
{
  [Serializable]
  public sealed class ResourceLibrary : IEnumerable<ResourceCollection>, IWriteContentsAsync
  {

    public static Func<Type, string[]> GetFolders;

    private Type _resourceType;
    private Type _buildType;

    [SerializeField, HideInInspector] private readonly string _resourceTypeName;
    [SerializeField, HideInInspector] private readonly string _buildTypeName;
    [SerializeField, HideInInspector] private readonly List<ResourceCollection> _collections = new();

    [SerializeField, ReadOnly] public readonly string Name;
    [SerializeField, HideInInspector] public readonly int Index;

    public Type ResourceType => _resourceType ??= Type.GetType(_resourceTypeName);
    public Type BuildType => _buildType ??= Type.GetType(_buildTypeName);
    public string AssetPath => $"{ResourceDatabase.AssetPath}/{Name}";
    public string ResourcePath => $"{ResourceDatabase.ResourcePath}/{Name}";
    public int IndicesCount => _collections.Count;

    /// <remarks> This only tallies the currently extant Resources.</remarks>
    [field: SerializeField, HideInInspector] public int Count { get; private set; }

    public ResourceCollection this[int id] => GetCollection(id);
    public ResourceCollection this[string name] => GetCollection(name);
    public ResourcePeek this[string collName, string resName] => GetResourcePeek(collName, resName);

    internal ResourceLibrary(Type resourceType, Type buildType, int index)
    {
      Name = resourceType.Name;
      _resourceTypeName = $"{resourceType.FullName}, {resourceType.Assembly.GetName().Name}";
      _buildTypeName = $"{buildType.FullName}, {buildType.Assembly.GetName().Name}";
      Index = index;
      Refresh();
    }

    public bool TryGetCollection(string name, out ResourceCollection coll)
    {
      foreach(ResourceCollection collection in _collections)
      {
        if(collection.Name == name)
        {
          coll = collection;
          return true;
        }
      }
      coll = null;
      return false;
    }

    public bool TryGetCollection(int id, out ResourceCollection coll)
    {
      int index = id.CollectionIndex();
      if(index >= 0 && index < _collections.Count && _collections[index] != null)
      {
        coll = _collections[index];
        return true;
      }
      coll = null;
      return false;
    }

    public bool TryGetResourcePath(int id, out string path)
    {
      if(TryGetCollection(id, out ResourceCollection collection))
      {
        return collection.TryGetResourcePath(id, out path);
      }
      path = null;
      return false;
    }

    public bool TryGetResourcePeek(int id, out ResourcePeek peek)
    {
      if(TryGetCollection(id, out ResourceCollection collection))
      {
        return collection.TryGetResourcePeek(id, out peek);
      }
      peek = null;
      return false;
    }

    public bool TryGetResourcePeek(string collName, string resName, out ResourcePeek peek)
    {
      if(TryGetCollection(collName, out ResourceCollection collection))
      {
        return collection.TryGetResourcePeek(resName, out peek);
      }
      peek = null;
      return false;
    }

    public bool TryGetResourceName(int id, out string name)
    {
      if(TryGetResourcePeek(id, out ResourcePeek peek))
      {
        name = peek.Name;
        return true;
      }
      name = null;
      return false;
    }

    public ResourceCollection GetCollection(string name)
    {
      if(!TryGetCollection(name, out ResourceCollection resl))
      {
        UnityDiagnostics.LogError(
          new Exception($"No ResourceCollection with name '{name}' found in ResourceLibrary '{Name}'"),
          stackTrace: new(true),
          id: ResourceDatabase.DiagnosticID + "missing_name.collection"
        );
      }
      return resl;
    }

    public ResourceCollection GetCollection(int id)
    {
      if(!TryGetCollection(id, out ResourceCollection resl))
      {
        UnityDiagnostics.LogError(
          new Exception($"No ResourceCollection for ID '{id.ToIDString(true)}' found in ResourceLibrary '{Name}'"),
          stackTrace: new(true),
          id: ResourceDatabase.DiagnosticID + "missing_id.collection"
        );
      }
      return resl;
    }

    public string GetResourcePath(int id) => GetCollection(id).GetResourcePath(id);
    public ResourcePeek GetResourcePeek(int id) => GetCollection(id).GetResourcePeek(id);
    public ResourcePeek GetResourcePeek(string collName, string resName) => GetCollection(collName).GetResourcePeek(resName);
    public string GetResourceName(int id) => GetCollection(id).GetResourceName(id);

    public bool IsValidID(int id)
    {
      if(TryGetCollection(id, out ResourceCollection collection))
      {
        return collection.IsValidID(id);
      }
      return false;
    }

    public void WriteContents(StreamWriter writer)
    {
      writer.WriteLine();
      writer.WriteLine($"||| Type: {ResourceType.Name}, Index: {Index}, Collections: {Count} |||");
      foreach(ResourceCollection collection in _collections)
      {
        collection.WriteContents(writer);
      }
    }

    public async Task WriteContentsAsync(StreamWriter writer)
    {
      await writer.WriteLineAsync();
      await writer.WriteLineAsync($"||| Type: {ResourceType.Name}, Index: {Index}, Collections: {Count} |||");
      foreach(ResourceCollection collection in _collections)
      {
        await collection.WriteContentsAsync(writer);
      }
    }

    public void AddCollection(string name)
    {
      if(!Application.isEditor) { return; }
      if(IndicesCount >= ResourceID.MaxCollections)
      {
        foreach(ResourceCollection collection in _collections)
        {
          if(collection != null && collection.Name == name){ return; }
        }
        _collections.Add(new ResourceCollection(name, _collections.Count, this));
        Count++;
      }
      else
      {
        UnityDiagnostics.LogError(
          new InvalidOperationException(GetIndexOverflowMessage(name)), 
          stackTrace: new(true),
          id: ResourceDatabase.DiagnosticID + ".max_collections"
         );
      }
    }

    public void RemoveCollection(string name)
    {
      if(!Application.isEditor) { return; }
      for(int i = 0; i < _collections.Count; i++)
      {
        if(_collections[i] != null && _collections[i].Name == name)
        {
          _collections[i] = null;
          Count--;
        }
      }
    }

    public string GetIndexOverflowMessage(string collectionName)
    {
      return $"Cannot create new Collection '{Name}/{collectionName}' ', {ResourceID.MaxCollections} existing indicies in Library. " +
        $"Consider rebuilding the Database to clear removed Collections.";
    }

    IEnumerator IEnumerable.GetEnumerator() => Enumeration.NonNullEnumerator(_collections);
    public IEnumerator<ResourceCollection> GetEnumerator() => Enumeration.NonNullEnumerator(_collections);

    internal void Refresh()
    {
      if(GetFolders == null || ResourceType == null)
      {
        UnityDiagnostics.LogMessage($"GetFolders null: {GetFolders == null}, ResourceType null: {ResourceType == null}");
        return;
      }
      string[] folders = GetFolders.Invoke(ResourceType);
      HashSet<string> extantCollections = new();
      for(int i = 0; i < _collections.Count; i++)
      {
        if(_collections[i] == null) { continue; }
        if(!folders.Contains(_collections[i].Name))
        {
          _collections[i] = null;
          Count--;
        }
        else
        {
          _collections[i].Refresh();
          extantCollections.Add(_collections[i].Name);
        }
      }
      foreach(string folder in folders)
      {
        if(!extantCollections.Contains(folder))
        {
          AddCollection(folder);
        }
      }
    }

  }
}