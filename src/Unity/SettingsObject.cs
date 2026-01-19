using UnityEngine;
using System;
using System.Reflection;


namespace WrathTools.Unity
{
  public abstract class SettingsObject : ScriptableObject
  {

    public static Func<Type, SettingsObject> GetSettings { get; set; } = t => Resources.Load<SettingsObject>($"{SettingsPath}/{t.Name}");
    public static string SettingsPath => "SettingsObjects";

    public abstract string DisplayName { get; }
    public abstract string CategoryName { get; }
    public abstract bool ShowInMenu { get; }

    protected static T Fetch<T>(ref T obj) where T : SettingsObject
    {
      if(obj == null)
      {
        obj = GetSettings.Invoke(typeof(T)) as T;
        if(obj == null)
        {
          UnityDiagnostics.LogError(
            new Exception($"Setttings object of Type '{typeof(T).Name}' was not found."),
            stackTrace: new(true)
          );
        }
      }
      return obj;
    }

    protected static void Unload<T>(ref T obj) where T : SettingsObject
    {
      if(obj != null)
      {
        Resources.UnloadAsset(obj);
        obj = null;
      }
    }

    public abstract void Merge(SettingsObject other);
    public abstract void Unload();

    public bool TryGetSetting<T>(string name, out T resl)
    {
      if(TryGetSettingProtected(name, out resl)) { return true; }
      Type thisType = GetType();
      PropertyInfo property = thisType.GetProperty(name, typeof(T));
      if(property != null)
      {
        resl = (T)property.GetValue(this);
        return true;
      }
      FieldInfo field = thisType.GetField(name);
      if(field != null && field.FieldType == typeof(T))
      {
        resl = (T)field.GetValue(this);
        return true;
      }
      return false;
    }

    protected virtual bool TryGetSettingProtected<T>(string name, out T resl)
    {
      resl = default;
      return false;
    }

  }
}