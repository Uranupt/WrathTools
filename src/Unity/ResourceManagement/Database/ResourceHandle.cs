using System;


namespace WrathTools.Unity.ResourceManagement
{ 
  public class ResourceHandle : IDisposable
  {

    private readonly bool _validOnCreated;

    public readonly int ID;
    public readonly Type ResourceType;
    public readonly Type BuildType;
    public bool Released { get; private set; }
    public bool IsValid => !Released && _validOnCreated;

    public ResourceObject Resource
    {
      get
      {
        if(!IsValid)
        {
          UnityDiagnostics.LogError(
             new InvalidOperationException("ResourceHandles should not be used while invalid or after being released"),
             stackTrace: new(true),
             id: ResourceDatabase.DiagnosticID + ".handle_invalid"
          );
          return null;
        }
        if(!TryGetResource(out ResourceObject resl))
        {
          UnityDiagnostics.LogError(
            new Exception($"Failed to find resource with ID: {ID} and Type: {ResourceType}"),
            stackTrace: new(true),
            id: ResourceDatabase.DiagnosticID + ".handle_missing_resource"
          );
        }
        return resl;
      }
    }

    public ResourceHandle(int id, Type buildType = null, bool exactBuildType = true, 
      Type resourceType = null, bool exactResourceType = true)
    {
      _validOnCreated = ValidityCheck(id, buildType, exactBuildType, resourceType, exactResourceType);
      if(!_validOnCreated)
      {
        ID = -1;
        Released = true;
        return;
      }
      ID = id;
      ResourceID.TryGetResourceType(id, out ResourceType);
      ResourceID.TryGetBuildType(id, out BuildType);
      ResourceCache.LogHandle(id);
      ResourceCache.Purged += OnPurge;
    }

    public bool TryGetResourceAs<T>(out T resource, bool exactType = true) where T : ResourceObject
    {
      if(!IsValid)
      {
        resource = null;
        return false;
      }
      return ResourceCache.TryGetResource<T>(ID, out resource, exactType);
    }

    public bool TryGetResourceAs(Type type, out ResourceObject resource, bool exactType = true)
    {
      if(!IsValid)
      {
        resource = null;
        return false;
      }
      return ResourceCache.TryGetResource(ID, type, out resource, exactType);
    }

    public bool TryGetResource(out ResourceObject resource)
    {
      if(!IsValid) 
      {
        resource = null;
        return false; 
      }
      return ResourceCache.TryGetResource(ID, ResourceType, out resource);
    }

    public ResourceHandle Duplicate()
    {
      if(!IsValid)
      {
        UnityDiagnostics.LogError(
          new InvalidOperationException($"Cannot duplicate an invalid ResourceHandle. Valid On Creation: {_validOnCreated}, Released: {Released}"),
          stackTrace: new(true),
          id: ResourceDatabase.DiagnosticID + ".handle_duplicate_invalid"
        );
      }
      return new ResourceHandle(ID);
    }

    public void Release()
    {
      if(Released) { return; }
      Released = true;
      ResourceCache.ReleaseHandle(ID);
      ResourceCache.Purged -= OnPurge;
    }

    public void Dispose() => Release();

    private void OnPurge()
    {
      ResourceCache.Purged -= OnPurge;
      Released = true;
    }

    private bool ValidityCheck(int id, Type buildType, bool exactBuildType, Type resourceType, bool exactResourceType)
    {
      if(resourceType == null && buildType == null)
      {
        return ResourceID.IsValid(id);
      }
      if(resourceType != null && !ResourceID.IsResourceType(id, resourceType, exactResourceType))
      { 
        return false; 
      }
      if(buildType != null && !ResourceID.IsBuildType(id, buildType, exactBuildType))
      {
        return false;
      }
      return true;
    }

  }
}
