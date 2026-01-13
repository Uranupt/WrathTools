using UnityEngine;
using System.Collections.Generic;

namespace WrathTools.Unity.ResourceManagement
{
  public class PrefabFetcher : SettingsObject<PrefabFetcher>
  {

    public override string DisplayName => "Prefabs";
    public override string CategoryName => "Resource Management";
    public override bool ShowInMenu => true;

    [SerializeField] private List<PrefabWrapper> _prefabs;
		[SerializeField] private List<PrefabBundle> _bundles;
    public static GameObject Get(string name)
    {
      foreach(PrefabWrapper wrapper in Instance._prefabs)
      {
        if(wrapper.Name == name)
        {
          return wrapper.Prefab;
        }
      }
			foreach(PrefabBundle bundle in Instance._bundles)
			{
				GameObject resl = bundle.Get(name);
				if(resl != null)
				{
					return resl;
				}
			}
      Debug.Log("No prefab found with name: " + name);
      return null;
    }

    public static GameObject GetFromBundle(string name, string bundleName)
		{
			foreach(PrefabBundle bundle in Instance._bundles)
			{
				if(bundle.Name == bundleName)
				{
					return bundle.Get(name);
				}
			}
			Debug.LogError("No bundle found with name: " + name);
			return null;
		}

    public static T Get<T>() where T : MonoBehaviour
		{
			return Get<T>(typeof(T).Name);
		}

    public static T Get<T>(string name) where T : MonoBehaviour
		{
			foreach(PrefabWrapper wrapper in Instance._prefabs)
			{
				if(wrapper.Name == name && wrapper.Prefab.TryGetComponent<T>(out T component))
				{
					return component;
				}
			}
			foreach(PrefabBundle bundle in Instance._bundles)
			{
				T resl = bundle.Get<T>(name);
				if(resl is not null)
				{
					return resl;
				}
			}
      Debug.LogError("No prefab found with component: "+ typeof(T).Name + " and name: " + name);
      return null;
    }

    public static T GetFromBundle<T>(string bundleName) where T : MonoBehaviour
		{
			return GetFromBundle<T>(typeof(T).Name, bundleName);
		}

    public static T GetFromBundle<T>(string name, string bundleName) where T : MonoBehaviour
		{
			foreach(PrefabBundle bundle in Instance._bundles)
			{
				if(bundle.Name == bundleName)
				{
					return bundle.Get<T>(name);
				}
			}
			Debug.Log("No bundle found with name: " + name);
			return null;
		}

    public override void Merge(PrefabFetcher other)
    {
      foreach(PrefabWrapper wrapper in other._prefabs)
      {
        _prefabs.Add(wrapper);
      }
      foreach(PrefabBundle bundle in other._bundles)
      {
        _bundles.Add(bundle);
      }
    }

    protected void OnValidate()
		{
			foreach(PrefabWrapper wrapper in _prefabs)
			{
				wrapper.Name = wrapper.Prefab.name;
			}
		}

  }

}