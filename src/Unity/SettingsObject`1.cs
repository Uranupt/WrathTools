using UnityEngine;


namespace WrathTools.Unity
{
  public abstract class SettingsObject<TSelf> : SettingsObject where TSelf : SettingsObject<TSelf>
  {

    private static TSelf _instance;

    public static TSelf Instance => Fetch(ref _instance);

    public override sealed void Merge(SettingsObject other)
    {
      if(other.GetType() == typeof(TSelf))
      {
        Merge(other as TSelf);
      }
    }

    public override sealed void Unload() => Unload(ref _instance);

    public abstract void Merge(TSelf other);

  }
}