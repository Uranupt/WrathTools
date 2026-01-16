using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using WrathTools.Unity;


namespace WrathTools.UnityEditor
{
  public static class SettingsObjectEditorHandler
  {

    public enum DuplicateResolutionChoice
    {
      Replace,
      Discard,
      Merge
    }

    private static readonly Dictionary<Type, SettingsObject> _settingsCache = new();

    [MenuItem("WrathTools/Resolve Duplicate Settings")]
    public static void ResolveDuplicateSettings()
    {
      foreach(Type type in _settingsCache.Keys)
      {
        string[] guids = AssetDatabase.FindAssets($"t:{type.Name}");
        string canonicalPath = $"{SettingsObject.SettingsPath}/{type.Name}.asset";
        List<string> dupePaths = guids.Select(AssetDatabase.GUIDToAssetPath).Where(p => p != canonicalPath).ToList();
        if(dupePaths.Count == 0) { continue; }

        SettingsObject canonical = GetSettingsOfType(type);
        foreach(string path in dupePaths)
        {
          SettingsObject other = AssetDatabase.LoadAssetAtPath(path, type) as SettingsObject;
          if(other == null) { continue; }
          DuplicateResolutionChoice choice = Prompt(type.Name, path);
          if(choice == DuplicateResolutionChoice.Replace)
          {
            EditorUtility.CopySerialized(other, canonical);
          }
          else if(choice == DuplicateResolutionChoice.Merge)
          {
            canonical.Merge(other);
          }
          AssetDatabase.DeleteAsset(path);
        }
        EditorUtility.SetDirty(canonical);
        AssetDatabase.SaveAssetIfDirty(canonical);
      }
    }

    [SettingsProviderGroup]
    public static SettingsProvider[] CreateProviders()
    {
      List<SettingsProvider> providers = new();
      foreach(SettingsObject obj in _settingsCache.Values)
      {
        if(!obj.ShowInMenu) { continue; }
        providers.Add(MakeProvider(obj));
      }
      return providers.ToArray();
    }

    [InitializeOnLoadMethod]
    private static void Initialize()
    {
      SettingsObject.GetSettings = GetSettingsOfType;
      EditorTools.EnsurePathExists($"Assets/Resources/{SettingsObject.SettingsPath}");
      BuildSettingsCache();
    }

    private static void BuildSettingsCache()
    {
      IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a => a.GetTypes())
        .Where(t => t.IsSubclassOf(typeof(SettingsObject)) && !t.IsAbstract);
      foreach(Type type in types)
      {
        _settingsCache[type] = GetSettingsOfType(type);
      }
    }

    private static DuplicateResolutionChoice Prompt(string typeName, string duplicatePath)
    {
      int choice = EditorUtility.DisplayDialogComplex(
        $"Duplicate {typeName}",
        $"Duplicate found at:\n{duplicatePath}\nHow would you like to handle the duplicate?",
        "Replace",
        "Discard",
        "Merge"
      );
      return (DuplicateResolutionChoice)choice;
    }

    private static SettingsObject GetSettingsOfType(Type type)
    {
      if(!type.IsSubclassOf(typeof(SettingsObject)) || type.IsAbstract) { return null; }
      MethodInfo createMethod = typeof(SettingsObjectEditorHandler)
        .GetMethod("GetSettings", BindingFlags.NonPublic | BindingFlags.Static)
        .MakeGenericMethod(type);
      return createMethod.Invoke(null, null) as SettingsObject;
    }

    private static T GetSettings<T>() where T : SettingsObject
    {
      T instance = Resources.Load<T>($"{SettingsObject.SettingsPath}/{typeof(T).Name}");
      if(instance == null)
      {
        instance = (T)ScriptableObject.CreateInstance(typeof(T));
        string path = $"Assets/Resources/{SettingsObject.SettingsPath}/{typeof(T).Name}.asset";
        AssetDatabase.CreateAsset(instance, path);
        EditorUtility.SetDirty(instance);
        AssetDatabase.SaveAssets();
      }
      return instance;
    }

    private static SettingsProvider MakeProvider(SettingsObject obj)
    {
      string settingsPath = $"{obj.CategoryName}/{obj.DisplayName}";
      return new SettingsProvider($"Project/{settingsPath}", SettingsScope.Project)
      {
        guiHandler = _ =>
        {
          SerializedObject serialized = new(obj);
          SerializedProperty prop = serialized.GetIterator();
          bool expanded = true;
          while(prop.NextVisible(expanded))
          {
            if(prop.name == "m_Script") { continue; }
            EditorGUILayout.PropertyField(prop, true);
            expanded = false;
          }
          serialized.ApplyModifiedProperties();
        }
      };
    }

  }
}