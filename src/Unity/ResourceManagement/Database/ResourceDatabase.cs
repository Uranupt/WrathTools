using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;


namespace WrathTools.Unity.ResourceManagement
{
  public class ResourceDatabase : SettingsObject<ResourceDatabase>, IEnumerable<ResourceLibrary>, IWriteContentsAsync
  {

    public static string AssetPath => "Assets/Resources/" + Instance._folder; 
    public static string ResourcePath => Instance._folder;
    public static string DumpPath => "Assets/ResourceDump";
    public static new string DiagnosticID => UnityDiagnostics.DiagnosticID + ".resource_database";

    public static event Action<ResourceLibrary> LibraryRemoved;
    public static event Action AutoUpdateTurnedOn;

    [SerializeField, HideInInspector] private string _folder = "ResourceDatabase"; //TODO: Move this to a Setting users can change and set up migration when it does
    [SerializeField, ReadOnly] private List<ResourceLibrary> _libraries = new();

    [SerializeField, HideInInspector] private bool _lastAutoUpdate = true;

    public override string DisplayName => "Database";
    public override string CategoryName => "Resource Management";
    public override bool ShowInMenu => true;
    public int IndicesCount => _libraries.Count;

    [field: SerializeField, HideInInspector] public int Count { get; private set; }

    [field: SerializeField, Tooltip("Whether to automatically update the Database")]
    public bool AutoUpdate { get; private set; } = true;

    [field: SerializeField, Tooltip("Whether to log a reminder Warning to the console on reload if Auto Update is off")]
    public bool WarnWhenUpdateIsOff { get; private set; } = true;

    [field: SerializeField, Tooltip("The default location for writing the text file contents of the Database")] 
    public string ContentFileWritePath { get; private set; }

    public ResourceLibrary this[int id] => GetLibrary(id);
    public ResourceLibrary this[string name] => GetLibrary(name);
    public ResourceLibrary this[Type type] => GetLibrary(type);
    public ResourceCollection this[string libName, string collName] => GetCollection(libName, collName);
    public ResourcePeek this[string libName, string collName, string resName] => GetResourcePeek(libName, collName, resName);

    #region TryGet Block

    public bool TryGetLibrary(string typeName, out ResourceLibrary library)
    {
      foreach(ResourceLibrary lib in this)
      {
        if(lib.Name == typeName)
        {
          library = lib;
          return true;
        }
      }
      library = null;
      return false;
    }

    public bool TryGetLibrary(Type type, out ResourceLibrary library) => TryGetLibrary(type.Name, out library);
    public bool TryGetLibrary<T>(out ResourceLibrary library) => TryGetLibrary(typeof(T), out library);

    public bool TryGetLibrary(int id, out ResourceLibrary library)
    {
      int index = id.LibraryIndex();
      if(index >= 0 && index < _libraries.Count && _libraries[index] != null)
      {
        library = _libraries[index];
        return true;
      }
      library = null;
      return false;
    }

    public bool TryGetCollection(int id, out ResourceCollection collection)
    {
      if(TryGetLibrary(id, out ResourceLibrary lib))
      {
        return lib.TryGetCollection(id, out collection);
      }
      collection = null;
      return false;
    }

    public bool TryGetCollection(string libName, string collName, out ResourceCollection collection)
    {
      if(TryGetLibrary(libName, out ResourceLibrary lib))
      {
        return lib.TryGetCollection(collName, out collection);
      }
      collection = null;
      return false;
    }

    public bool TryGetResourcePath(int id, out string path)
    {
      if(TryGetCollection(id, out ResourceCollection coll))
      {
        return coll.TryGetResourcePath(id, out path);
      }
      path = null;
      return false;
    }

    public bool TryGetResourcePeek(int id, out ResourcePeek peek)
    {
      if(TryGetCollection(id, out ResourceCollection coll))
      {
        return coll.TryGetResourcePeek(id, out peek);
      }
      peek = null;
      return false;
    }

    public bool TryGetResourcePeek(string libName, string collName, string resName, out ResourcePeek peek)
    {
      if(TryGetCollection(libName, collName, out ResourceCollection coll))
      {
        return coll.TryGetResourcePeek(resName, out peek);
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

    public bool TryGetBuildType(int id, out Type type)
    {
      if(TryGetLibrary(id, out ResourceLibrary lib))
      {
        type = lib.BuildType;
        return true;
      }
      type = null;
      return false;
    }

    public bool TryGetResourceType(int id, out Type type)
    {
      if(TryGetLibrary(id, out ResourceLibrary lib))
      {
        type = lib.ResourceType;
        return true;
      }
      type = null;
      return false;
    }

    #endregion

    #region Get Block

    public ResourceLibrary GetLibrary(string typeName)
    {
      if(!TryGetLibrary(typeName, out ResourceLibrary resl))
      {
        UnityDiagnostics.LogError(
          new Exception($"No ResourceLibrary for the ResourceObject Type '{typeName}' was found in the ResourceDatabase."),
          stackTrace: new(true),
          id: DiagnosticID + ".missing_resource_type"
        );
      }
      return resl;
    }

    public ResourceLibrary GetLibrary(int id)
    {
      if(!TryGetLibrary(id, out ResourceLibrary resl))
      {
        UnityDiagnostics.LogError(
          new Exception($"No ResourceLibrary for the ID '{id.ToIDString(true)}' was found in the ResourceDatabase"),
          stackTrace: new(true),
          id: DiagnosticID + ".missing_id.library"
        );
      }
      return resl;
    }

    public ResourceLibrary GetLibrary(Type type) => GetLibrary(type.Name);
    public ResourceLibrary GetLibrary<T>() => GetLibrary(typeof(T));
    public ResourceCollection GetCollection(int id) => GetLibrary(id).GetCollection(id);
    public ResourceCollection GetCollection(string libName, string collName) => GetLibrary(libName).GetCollection(collName);
    public string GetResourcePath(int id) => GetCollection(id).GetResourcePath(id);
    public ResourcePeek GetResourcePeek(int id) => GetCollection(id).GetResourcePeek(id);
    public ResourcePeek GetResourcePeek(string libName, string collName, string resName) => GetCollection(libName, collName).GetResourcePeek(resName);
    public string GetResourceName(int id) => GetCollection(id).GetResourceName(id);
    public Type GetBuildType(int id) => GetLibrary(id).BuildType;
    public Type GetResourceType(int id) => GetLibrary(id).ResourceType;

    #endregion

    public bool IsValidID(int id) => TryGetLibrary(id, out _);

    public bool IDIsResourceType(int id, Type type)
    {
      return TryGetResourceType(id, out Type libType) && libType == type;
    }

    public bool IDIsBuildType(int id, Type type)
    {
      return TryGetBuildType(id, out Type libType) && libType == type;
    }

    public override void Merge(ResourceDatabase other)
    {
     //For right now I'm leaving this blank, any meaningful merge would mean the Resources are located at a different root folder and would have to be moved
     //or deleted depending on handling case. Should be a "You broke it yourself" situation if a duplicate does arise, only reasonable case I forsee is someone
     //copy-pasting one from another project.
    }

    public void WriteContents(StreamWriter writer)
    {
      writer.WriteLine($"=== ResourceDatabase Contents for {Count} Libraries ===");
      foreach(ResourceLibrary library in this)
      {
        library.WriteContents(writer);
      }
    }

    public async Task WriteContentsAsync(StreamWriter writer)
    {
      await writer.WriteLineAsync($"=== ResourceDatabase Contents for {Count} Libraries ===");
      foreach(ResourceLibrary library in this)
      {
        await library.WriteContentsAsync(writer);
      }
    }

    public bool VerifyLibraries(IEnumerable<(Type, Type)> knownTypes)
    {
      if(!Application.isEditor) { return false; }
      bool changed = false;
      HashSet<string> seenTypes = new();
      foreach((Type resourceType, Type buildType) in knownTypes)
      {
        seenTypes.Add(resourceType.Name);
        if(!TryGetLibrary(resourceType, out _))
        {
          NewLibrary(resourceType, buildType);
          changed = true;
        }
      }
      for(int i = 0; i < _libraries.Count; i++)
      {
        if(_libraries[i] != null && !seenTypes.Contains(_libraries[i].Name))
        {
          LibraryRemoved?.Invoke(_libraries[i]);
          _libraries[i] = null;
          changed = true;
        }
      }
      return changed;
    }

    public void Refresh(IEnumerable<(Type, Type)> knownTypes)
    {
      if(!Application.isEditor) { return; }
      VerifyLibraries(knownTypes);
      foreach(ResourceLibrary lib in this)
      {
        lib?.Refresh();
      }
    }

    public void Rebuild(IEnumerable<(Type, Type)> knownTypes)
    {
      if(!Application.isEditor) { return; }
      _libraries.Clear();
      Refresh(knownTypes);
    }

    IEnumerator IEnumerable.GetEnumerator() => Enumeration.NonNullEnumerator(_libraries);
    public IEnumerator<ResourceLibrary> GetEnumerator() => Enumeration.NonNullEnumerator(_libraries);


    private void NewLibrary(Type resourceType, Type buildType)
    {
      if(_libraries.Count >= ResourceID.MaxLibraries)
      {
        UnityDiagnostics.LogWarning(
          $"Cannot create a new ResourceLibrary for ResourceObject Type '{resourceType.Name}', " +
            $"the Database already has {ResourceID.MaxLibraries} indicies. You will need to Rebuild the Database.",
          id: $"warning.{DiagnosticID}.max_libraries"
        );
        return;
      }
      _libraries.Add(new ResourceLibrary(resourceType, buildType, _libraries.Count));
      Count++;
    }

    private void OnValidate()
    {
      if(AutoUpdate != _lastAutoUpdate)
      {
        _lastAutoUpdate = AutoUpdate;
        if(AutoUpdate == true)
        {
          AutoUpdateTurnedOn?.Invoke();
        }
      }
    }

  }
}