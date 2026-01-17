using System;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using WrathTools.Unity.ResourceManagement;
using WrathTools.Unity;


namespace WrathTools.UnityEditor.ResourceManagement
{
  public class ResourceFolderEnforcer : AssetModificationProcessor
  {

    private enum PathType
    {
      AssetOrMissing,
      Unrelated,
      Database,
      Library,
      Collection,
      Subcollection
    }

    private class PathAnalysisResult
    {

      public readonly PathType PathType;
      public readonly string DeepestFolder;
      public readonly string FullPath;
      public readonly ResourceLibrary Library;
      public readonly bool HasSubfolders;

      public PathAnalysisResult(
        PathType pathType,
        string deepestFolder,
        string fullPath,
        bool hasSubfolders = false,
        ResourceLibrary library = null
      )
      {
        this.PathType = pathType;
        DeepestFolder = deepestFolder;
        FullPath = fullPath;
        HasSubfolders = hasSubfolders;
        Library = library;
      }

    }

    private static bool _ignore = false;

    public static void OnWillCreateAsset(string assetPath)
    {
      if(_ignore) { return; }
      PathAnalysisResult pathAnalysis = AnalyzePath(assetPath.Replace(".meta", ""));
      switch(pathAnalysis.PathType)
      {
        case PathType.AssetOrMissing:
        case PathType.Unrelated:
        {
          return;
        }
        case PathType.Collection:
        {
          if(!ResourceDatabase.Instance.AutoUpdate) { return; }
          if(pathAnalysis.Library == null)
          {
            Debug.LogError($"The ResourceLibrary for the new Collection folder at '{pathAnalysis.FullPath}' could not be found in the Database." +
              $" New Collection folder will be deleted. If issue persists, Database rebuild is recommended.");
            Try(() => AssetDatabase.DeleteAsset(pathAnalysis.FullPath));
            return;
          }
          if(pathAnalysis.Library.IndicesCount >= ResourceID.MaxCollections)
          {
            pathAnalysis.Library.AddCollection(pathAnalysis.DeepestFolder);
            EditorApplication.delayCall += DelayedSave;
          }
          else
          {
            Debug.LogError(pathAnalysis.Library.GetIndexOverflowMessage(pathAnalysis.DeepestFolder));
            Try(() => AssetDatabase.DeleteAsset(pathAnalysis.FullPath));
          }
          break;
        }
        default:
        {
          Debug.LogError($"New folder at '{pathAnalysis.FullPath}' created at an invalid depth. Only Collections (subfolders of Libraries) can be manually created.");
          Try(() => AssetDatabase.DeleteAsset(pathAnalysis.FullPath));
          break;
        }
      }
    }

    public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions _)
    {
      if(_ignore) { return AssetDeleteResult.DidNotDelete; }
      PathAnalysisResult pathAnalysis = AnalyzePath(assetPath);
      switch(pathAnalysis.PathType)
      {
        case PathType.Database:
        case PathType.Library:
        {
          Debug.LogError($"Users should not manually delete the Database or Library folder at '{pathAnalysis.FullPath}'");
          return AssetDeleteResult.FailedDelete;
        }
        case PathType.Collection:
        {
          if(!ResourceDatabase.Instance.AutoUpdate) { return AssetDeleteResult.DidNotDelete;  }
          pathAnalysis.Library?.RemoveCollection(pathAnalysis.DeepestFolder);
          EditorApplication.delayCall += DelayedSave;
          break;
        }
      }
      return AssetDeleteResult.DidNotDelete;
    }

    public static AssetMoveResult OnWillMoveAsset(string oldPath, string newPath)
    {
      if(_ignore) { return AssetMoveResult.DidNotMove; }
      PathAnalysisResult oldPathAnalysis = AnalyzePath(oldPath);
      PathAnalysisResult newPathAnalysis = AnalyzePath(newPath, true);

      if(oldPathAnalysis.PathType == PathType.Database
        || newPathAnalysis.PathType == PathType.Database
        || oldPathAnalysis.PathType == PathType.Library
        || newPathAnalysis.PathType == PathType.Library)
      {
        Debug.LogError($"Users should not manually move the Database or Library folders. Prevented folder at '{oldPathAnalysis.FullPath}'" +
          $" from moving to '{newPathAnalysis.FullPath}'");
        return AssetMoveResult.FailedMove;
      }

      if(oldPathAnalysis.PathType == PathType.AssetOrMissing
        || oldPathAnalysis.PathType == PathType.Subcollection)
      {
        return AssetMoveResult.DidNotMove;
      }

      if(newPathAnalysis.PathType == PathType.Subcollection)
      {
        Debug.LogError($"Cannot move folder at '{oldPathAnalysis.FullPath}' to '{newPathAnalysis.FullPath}', " +
          $"ResourceCollection folders cannot have subfolders.");
        return AssetMoveResult.FailedMove;
      }
      
      static bool CanAddCollection(PathAnalysisResult analysisResult)
      {
        if(analysisResult.Library != null && analysisResult.Library.IndicesCount >= ResourceID.MaxCollections)
        {
          Debug.LogError($"Cannot move folder to {analysisResult.FullPath}, the Library cannot log any new Collections.");
          return false;
        }
        return true;
      }

      (bool, bool) toFromCollection = 
        (oldPathAnalysis.PathType == PathType.Collection, newPathAnalysis.PathType == PathType.Collection);
      switch(toFromCollection)
      {
        case (false, false): //Unrelated -> Unrelated
        {
          return AssetMoveResult.DidNotMove;
        }
        case (false, true): //Unrelated -> Collection
        {
          if(oldPathAnalysis.HasSubfolders)
          {
            Debug.LogError($"Cannot move folder at '{oldPathAnalysis.FullPath}' to '{newPathAnalysis.FullPath}', " +
              $"folder contains subfolders and destination is a ResourceCollection path.");
            return AssetMoveResult.FailedMove;
          }
          if(ResourceDatabase.Instance.AutoUpdate)
          {
            if(!CanAddCollection(newPathAnalysis))
            {
              return AssetMoveResult.FailedMove;
            }
            EditorApplication.delayCall += () => DelayedAddCollection(newPathAnalysis.Library, newPathAnalysis.DeepestFolder);
          }
          return AssetMoveResult.DidNotMove;
        }
        case (true, false): //Collection -> Unrelated
        {
          if(ResourceDatabase.Instance.AutoUpdate)
          {
            oldPathAnalysis.Library?.RemoveCollection(oldPathAnalysis.DeepestFolder);
            EditorApplication.delayCall += DelayedSave;
          }
          return AssetMoveResult.DidNotMove;
        }
        case (true, true): //Collection -> Collection
        {
          if(ResourceDatabase.Instance.AutoUpdate)
          {
            if(!CanAddCollection(newPathAnalysis))
            {
              return AssetMoveResult.FailedMove;
            }
            oldPathAnalysis.Library?.RemoveCollection(oldPathAnalysis.DeepestFolder);
            EditorApplication.delayCall += DelayedSave;
            EditorApplication.delayCall += () => DelayedAddCollection(newPathAnalysis.Library, newPathAnalysis.DeepestFolder);
          }
          return AssetMoveResult.DidNotMove;
        }
      }
    }

    internal static void EnsureDatabaseFolderExists()
    {
      Try(() => EditorTools.EnsurePathExists(ResourceDatabase.AssetPath));
    }

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
      ResourceLibrary.GetFolders = GetFolders;
      ResourceDatabase.LibraryRemoved += OnLibraryRemoved;
    }

    private static void IgnoreWrapper(Action action)
    {
      _ignore = true;
      action.Invoke();
      _ignore = false;
    }

    private static bool Try(Action action)
    {
      _ignore = true;
      return Diagnostics.Try(action, e => new UnityErrorContext(e, stackTrace: new(true)), onFinally: () => _ignore = false);
    }

    private static string[] GetFolders(Type type)
    {
      EnsureLibraryFolderExists(type);
      string path = $"{ResourceDatabase.AssetPath}/{type.Name}";
      return AssetDatabase.GetSubFolders(path)
        .Select(f => f.Replace(path + "/", ""))
        .ToArray();
    }

    private static void EnsureLibraryFolderExists(Type type)
    {
      string path = $"{ResourceDatabase.AssetPath}/{type.Name}";
      if(!AssetDatabase.IsValidFolder(path))
      {
        Try(() => AssetDatabase.CreateFolder(ResourceDatabase.AssetPath, type.Name));
      }
    }

    private static PathAnalysisResult AnalyzePath(string assetPath, bool predict = false)
    {
      if(assetPath == null || (!predict && !AssetDatabase.IsValidFolder(assetPath)))
      {
        return new PathAnalysisResult(PathType.AssetOrMissing, "", assetPath);
      }
      bool hasSubfolders = !predict && AssetDatabase.GetSubFolders(assetPath).Length > 0;
      string[] parts = assetPath.Split('/');
      string currPath = parts[0];
      for(int i = 1; i < parts.Length; i++)
      {
        currPath += $"/{parts[i]}";
        if(currPath == ResourceDatabase.AssetPath)
        {
          int remainder = parts.Length - (i + 1);
          remainder = remainder <= 3 ? remainder : 3;
          PathType pathType = (PathType)(remainder + 2);
          ResourceLibrary library = null;
          if(remainder > 0)
          {
            ResourceDatabase.Instance.TryGetLibrary(parts[i + 1], out library);
          }
          return new PathAnalysisResult(pathType, parts[^1], assetPath, hasSubfolders, library);
        }
      }
      return new PathAnalysisResult(PathType.Unrelated, parts[^1], assetPath, hasSubfolders);
    }

    private static void DelayedSave()
    {
      _ignore = true;
      EditorUtility.SetDirty(ResourceDatabase.Instance);
      AssetDatabase.SaveAssetIfDirty(ResourceDatabase.Instance);
      _ignore = false;
    }

    private static void DelayedAddCollection(ResourceLibrary library, string name)
    {
      if(Try(() => library?.AddCollection(name)))
      {
        EditorUtility.SetDirty(ResourceDatabase.Instance);
        AssetDatabase.SaveAssetIfDirty(ResourceDatabase.Instance);
      }
    }

    private static void OnLibraryRemoved(ResourceLibrary library)
    {
      if(!AssetDatabase.IsValidFolder(library.AssetPath)) { return; }
      bool delete = EditorUtility.DisplayDialog("ResourceLibrary Removed",
        $"The ResourceObject Type '{library.Name}' has been removed. Do you want to delete the contents of the Library's folder or " +
          $"dump them at: {ResourceDatabase.DumpPath}?",
        "Delete",
        "Dump"
      );
      if(delete)
      {
        AssetDatabase.DeleteAsset(library.AssetPath);
      }
      else
      {
        EditorTools.EnsurePathExists(ResourceDatabase.DumpPath);
        AssetDatabase.MoveAsset(library.AssetPath, ResourceDatabase.DumpPath);
      }

    }

  }
}