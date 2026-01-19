using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;


namespace WrathTools.Unity.ResourceManagement
{
  [Serializable]
  public sealed class ResourceCollection : IEnumerable<ResourcePeek>, IWriteContentsAsync
  {

    [SerializeReference, HideInInspector] private readonly List<ResourcePeek> _resourcePeeks = new();
    [SerializeReference, HideInInspector] public readonly ResourceLibrary Library;
    [SerializeField, HideInInspector] public readonly string Name;
    [SerializeField, HideInInspector] public readonly int Index;

    public Type ResourceType => Library.ResourceType;
    public Type BuildType => Library.BuildType;
    public string ResourcePath => $"{Library.ResourcePath}/{Name}";
    public string AssetPath => $"{Library.AssetPath}/{Name}";
    public int IndicesCount => _resourcePeeks.Count;

    /// <remarks> This only tallies the currently extant Resources.</remarks>
    [field: SerializeField, HideInInspector] public int Count { get; private set; }

    public ResourcePeek this[int id] => GetResourcePeek(id);
    public ResourcePeek this[string name] => GetResourcePeek(name);

    internal ResourceCollection(string name, int index, ResourceLibrary library)
    {
      Name = name;
      Index = index;
      Library = library;
      Refresh();
    }

    public bool TryGetResourcePeek(int id, out ResourcePeek peek)
    {
      int index = id.ResourceIndex();
      if(index >= 0 && index < _resourcePeeks.Count && _resourcePeeks[index] != null)
      {
        peek = _resourcePeeks[index];
        return true;
      }
      peek = null;
      return false;
    }

    public bool TryGetResourcePeek(string name, out ResourcePeek peek)
    {
      foreach(ResourcePeek pk in this)
      {
        if(pk.Name == name)
        {
          peek = pk;
          return true;
        }
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

    public bool TryGetResourcePath(int id, out string path)
    {
      if(TryGetResourceName(id, out string name))
      {
        path = $"{ResourcePath}/{name}";
        return true;
      }
      path = null;
      return false;
    }

    public ResourcePeek GetResourcePeek(int id)
    {
      if(!TryGetResourcePeek(id, out ResourcePeek peek))
      {
        UnityDiagnostics.LogError(
          new Exception($"No ResourceObject with the ID '{id.ToIDString(true)}' was found in the ResourceCollection '{Library.Name}/{Name}'"),
          stackTrace: new(true)
        );
      }
      return peek;
    }

    public ResourcePeek GetResourcePeek(string name)
    {
      if(!TryGetResourcePeek(name, out ResourcePeek peek))
      {
        UnityDiagnostics.LogError(
          new Exception($"No ResourceObject with the name '{name}' was found in the ResourceCollection '{Library.Name}/{Name}'"),
          stackTrace: new(true)
        );
      }
      return peek;
    }

    public string GetResourceName(int id) => GetResourcePeek(id).Name;
    public string GetResourcePath(int id) => $"{ResourcePath}/{GetResourceName(id)}";

    public bool IsValidID(int id) => TryGetResourcePeek(id, out _);

    public void UpdatePeek(ResourceObject resource)
    {
      if(!Application.isEditor) { return; }
      if(resource == null || resource.GetType() != ResourceType){ return; }
      if(resource.ID < 0)
      {
        LogNewResource(resource);
      }
      else if(resource.ID.CollectionIndex() != Index)
      {
        if(ResourceDatabase.Instance.TryGetCollection(resource.ID, out ResourceCollection old))
        {
          old.RemovePeek(resource.ID);
        }
        LogNewResource(resource);
      }
      else
      {
        ResourcePeek peek = resource.GetPeek();
        peek.Name = resource.name;
        peek.ID = resource.ID;
        _resourcePeeks[resource.ID.ResourceIndex()] = peek;
      }
    }

    public void RemoveResource(ResourceObject resource)
    {
      if(!Application.isEditor) { return; }
      if(resource.ID < 0 || resource.ID.CollectionIndex() != Index) { return; }
      RemovePeek(resource.ID);
    }

    IEnumerator IEnumerable.GetEnumerator() => Enumeration.NonNullEnumerator(_resourcePeeks);
    public IEnumerator<ResourcePeek> GetEnumerator() => Enumeration.NonNullEnumerator(_resourcePeeks);

    public void WriteContents(StreamWriter writer)
    {
      writer.WriteLine();
      writer.WriteLine($"-+- Collection: {Name}, Index: {Index}, Resources: {Count} -+-");
      foreach(ResourcePeek peek in this)
      {
        writer.WriteLine($"{peek.ID.ToIDString()}: {peek.Name}");
      }
    }

    public async Task WriteContentsAsync(StreamWriter writer)
    {
      await writer.WriteLineAsync();
      await writer.WriteLineAsync($"-+- Collection: {Name}, Index: {Index}, Resources: {Count} -+-");
      foreach(ResourcePeek peek in this)
      {
        await writer.WriteLineAsync($"{peek.ID.ToIDString()}: {peek.Name}");
      }
    }

    internal void Refresh()
    {
      ResourceObject[] resources = Resources.LoadAll(ResourcePath, ResourceType) as ResourceObject[];
      HashSet<int> extantIDs = new(resources.Where(r => r.ID >= 0).Select(r => r.ID.ResourceIndex()));
      //Pruning
      for(int i = 0; i < _resourcePeeks.Count; i++)
      {
        if(_resourcePeeks[i] == null || extantIDs.Contains(i)) { continue; }
        _resourcePeeks[i] = null;
        Count--;
      }
      //Discovery
      foreach(ResourceObject resource in resources)
      {
        UpdatePeek(resource);
      }
    }

    private void LogNewResource(ResourceObject resource)
    {
      if(IndicesCount < ResourceID.MaxResources)
      {
        resource.SetID(ResourceID.Build(Library.Index, Index, _resourcePeeks.Count));
        ResourcePeek peek = resource.GetPeek();
        peek.Name = resource.name;
        peek.ID = resource.ID;
        _resourcePeeks.Add(peek);
        Count++;
      }
      else
      {
        UnityDiagnostics.LogError(new Exception($"Cannot log ResourceObject with name '{resource.name}' in Collection '{Library.Name}/{Name}'" 
          + $", it already has {ResourceID.MaxResources} existing indices. Consider a new Collection, or rebuilding the Database to clear removed Resource IDs."));
      }
    }

    private void RemovePeek(int id)
    {
      int index = id.ResourceIndex();
      if(index >= _resourcePeeks.Count || _resourcePeeks[index] == null) { return; }
      _resourcePeeks[index] = null;
      Count--;
    }

  }
}
