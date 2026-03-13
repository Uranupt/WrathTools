using UnityEditor;
using System.Collections.Generic;
using WrathTools.Unity.ResourceManagement;


namespace WrathTools.UnityEditor.ResourceManagement
{
  public class ResourceEnforcer : AssetModificationProcessor
  {

    private static readonly HashSet<ResourceObject> _updatedResources = new();

    public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions _)
    {
      if(!ResourceDatabase.Instance.AutoUpdate) { return AssetDeleteResult.DidNotDelete; }
      UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
      if(asset != null && asset is ResourceObject resource 
        && ResourceDatabase.Instance.TryGetCollection(resource.ID, out ResourceCollection collection))
      {
        collection.RemoveResource(resource);
      }
      EditorUtility.SetDirty(ResourceDatabase.Instance);
      AssetDatabase.SaveAssetIfDirty(ResourceDatabase.Instance);
      return AssetDeleteResult.DidNotDelete;
    }

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
      //TODO: Central static class to declare initialize dependencies
      ResourceFolderEnforcer.Initialize();
      SettingsObjectEditorHandler.Initialize();
      ResourceObject.ResourceIDChanged += OnResourceIDChanged;
      ResourceObject.ResourceValidated += OnResourceValidate;
    }

    private static void OnResourceValidate(ResourceObject resource)
    {
      if(!ResourceDatabase.Instance.AutoUpdate) { return; }
      _updatedResources.Add(resource);
      EditorApplication.delayCall -= ResolveResourceUpdates;
      EditorApplication.delayCall += ResolveResourceUpdates;
    }

    private static void OnResourceIDChanged(ResourceObject resource)
    {
      EditorUtility.SetDirty(resource);
      AssetDatabase.SaveAssetIfDirty(resource);
    }

    private static bool TryFindCollection(string path, out ResourceCollection collection)
    {
      string[] parts = path.Split('/');
      if(parts.Length <= 3) //Minimum of Library -> Collection -> Resource
      {
        collection = null;
        return false;
      }
      string currPath = parts[0];
      for(int i = 1; i < parts.Length; i++)
      {
        if(currPath == ResourceDatabase.AssetPath)
        {
          if(ResourceDatabase.Instance.TryGetLibrary(parts[i], out ResourceLibrary lib))
          {
            return lib.TryGetCollection(parts[i + 1], out collection);
          }
          else
          {
            break;
          }
        }
        currPath += $"/{parts[i]}";
      }
      collection = null;
      return false;
    }

    private static void ResolveResourceUpdates()
    {
      ResourceObject.ResourceValidated -= OnResourceValidate;
      foreach(ResourceObject resource in _updatedResources)
      {
        if(TryFindCollection(AssetDatabase.GetAssetPath(resource), out ResourceCollection collection))
        {
          collection.UpdatePeek(resource);
        }
        else if(ResourceDatabase.Instance.TryGetCollection(resource.ID, out collection))
        {
          collection.RemoveResource(resource);
        }
      }
      EditorUtility.SetDirty(ResourceDatabase.Instance);
      AssetDatabase.SaveAssetIfDirty(ResourceDatabase.Instance);
      _updatedResources.Clear();
      ResourceObject.ResourceValidated += OnResourceValidate;
    }

  }
}
