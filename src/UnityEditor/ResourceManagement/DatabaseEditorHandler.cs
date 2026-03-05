using System.IO;
using UnityEditor;
using System.Linq;
using System;
using System.Collections.Generic;
using WrathTools.Unity.ResourceManagement;
using WrathTools.Unity;


namespace WrathTools.UnityEditor.ResourceManagement
{ 
  public static class DatabaseEditorHandler
  {

    private static readonly HashSet<(Type, Type)> _resourceTypes = new();

    [MenuItem("WrathTools/Database/Refresh Database")]
    public static void Refresh()
    {
      ResourceDatabase.Instance.Refresh(_resourceTypes);
      EditorUtility.SetDirty(ResourceDatabase.Instance);
      AssetDatabase.SaveAssetIfDirty(ResourceDatabase.Instance);
    }

    [MenuItem("WrathTools/Database/Rebuild Database (Hard Reset)")]
    public static void Rebuild()
    {
      bool response = EditorUtility.DisplayDialog(
        "Rebuild Database",
        "This will purge all entries in the Database and rebuild them from scratch. Existing saves and IDs will no longer be valid. " +
          "\n Are you sure?",
        "Yes, I'm Sure",
        "No, Cancel"
      );
      if(!response) { return; }
      ResourceDatabase.Instance.Rebuild(_resourceTypes);
      EditorUtility.SetDirty(ResourceDatabase.Instance);
      AssetDatabase.SaveAssetIfDirty(ResourceDatabase.Instance);
    }

    [MenuItem("WrathTools/Database/Write IDs To File")]
    public static void PrintIDs()
    {
      TextInputPopup.Create("Enter path to write ResourceDatabase contents at", WriteToPath, ResourceDatabase.Instance.ContentFileWritePath);
    }

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
      ResourceFolderEnforcer.EnsureDatabaseFolderExists();
      BuildTypeCache();
      ResourceDatabase.AutoUpdateTurnedOn += Refresh;
      if(ResourceDatabase.Instance.AutoUpdate)
      {
        if(ResourceDatabase.Instance.VerifyLibraries(_resourceTypes))
        {
          EditorUtility.SetDirty(ResourceDatabase.Instance);
          AssetDatabase.SaveAssetIfDirty(ResourceDatabase.Instance);
        }
      }
      else if(ResourceDatabase.Instance.WarnWhenUpdateIsOff)
      {
        UnityDiagnostics.LogWarning(
          "ResourceDatabase's 'Auto Update' feature is turned off. You must manually update with the menu command WrathTools/Database/Refresh Database."
          + " You can disable this warning by unchecking 'Warn When Update Is Off' in the Database settings.",
          id: $"warning.{ResourceDatabase.DiagnosticID}.auto_update_off"
        );
      }
    }

    private static void BuildTypeCache()
    {
      _resourceTypes.Clear();
      IEnumerable<Type> types = TypeCache.GetTypesDerivedFrom<ResourceObject>()
        .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition);
      foreach(Type type in types)
      {
        if(TryGetBuildType(type, out Type buildType))
        {
          _resourceTypes.Add((type, buildType));
        }
      }
    }

    private static bool TryGetBuildType(Type type, out Type resl)
    {
      while(type != null && type != typeof(object))
      {
        if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ResourceObject<>))
        {
          Type[] args = type.GetGenericArguments();
          if(args.Length == 1)
          {
            resl = args[0];
            return true;
          }
        }
        type = type.BaseType;
      }
      resl = null;
      return false;
    }

    private static void WriteToPath(string path)
    {
      Serialization.WriteToFile(ResourceDatabase.Instance, Path.Combine(path, "ResourceDatabase.txt"));
    }

  }
}
