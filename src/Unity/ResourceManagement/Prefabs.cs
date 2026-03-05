using UnityEngine;
using System.Collections.Generic;
using System;

namespace WrathTools.Unity.ResourceManagement
{
  public class Prefabs : SettingsObject<Prefabs>
  {

		public static string DiagnosticID => UnityDiagnostics.DiagnosticID + ".prefabs";

    public override string DisplayName => "Prefabs";
    public override string CategoryName => "Resource Management";
    public override bool ShowInMenu => true;

    [SerializeField] private List<PrefabWrapper> _prefabs;
		[SerializeField] private List<PrefabBundle> _bundles;

		public static bool TryGet(string name, out GameObject gameObject)
		{
			if(Instance.TryRetrieve(name, out GameObject obj))
			{
				gameObject = GameObject.Instantiate(obj);
				return true;
			}
			gameObject = null;
			return false;
		}

		public static bool TryGetTemp(string name, out TempGameObject tempObject)
		{
			if(Instance.TryRetrieve(name, out GameObject obj))
			{
				tempObject = new TempGameObject(obj);
				return true;
			}
			tempObject = null;
			return false;
		}

		public static bool TryGet<T>(out T component, string name = null) where T : Component
		{
			if(Instance.TryRetrieve(name ?? typeof(T).Name, out T comp))
			{
				component = GameObject.Instantiate(comp);
				return true;
			}
			component = null;
			return false;
		}

		public static bool TryGetTemp<T>(out TempComponent<T> tempComponent, string name = null) where T : Component
		{
			if(Instance.TryRetrieve(name ?? typeof(T).Name, out T comp))
			{
				tempComponent = new TempComponent<T>(comp);
				return true;
			}
			tempComponent = null;
			return false;
		}

		public static GameObject Get(string name)
		{
			if(!TryGet(name, out GameObject obj))
			{
        UnityDiagnostics.LogError(
					new Exception($"No Prefab found with name '{name}'"), 
					stackTrace: new(true),
					id: DiagnosticID + ".missing_prefab.direct"
				);
			}
      return obj;
    }

		public static TempGameObject GetTemp(string name)
		{
			if(!TryGetTemp(name, out TempGameObject obj))
			{
        UnityDiagnostics.LogError(
					new Exception($"No Prefab found with name '{name}'"), 
					stackTrace: new(true),
					id: DiagnosticID + ".missing_prefab.temp"
				);
      }
      return obj;
    }

		public static T Get<T>(string name = null) where T : Component
		{
			if(!TryGet(out T comp, name))
			{
				UnityDiagnostics.LogError(
					new Exception($"No Prefab found with name '{name}' and a Component of Type '{typeof(T).Name}'"),
					stackTrace: new(true),
					id: DiagnosticID + ".missing_component.direct"
				);
			}
      return comp;

		}

		public static TempComponent<T> GetTemp<T>(string name = null) where T : Component
		{
			if(!TryGetTemp(out TempComponent<T> comp, name))
			{
				UnityDiagnostics.LogError(
					new Exception($"No Prefab found with name '{name}' and a Component of Type '{typeof(T).Name}'"),
					stackTrace: new(true),
					id: DiagnosticID + ".missing_component.temp"
				);
			}
      return comp;
    }

    public override void Merge(Prefabs other)
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

    private void OnValidate()
		{
			foreach(PrefabWrapper wrapper in _prefabs)
			{
				wrapper.Name = wrapper.Prefab.name;
			}
		}

		private bool TryRetrieve(string name, out GameObject gameObject)
		{
			foreach(PrefabWrapper wrapper in _prefabs)
			{
				if(wrapper.Name == name)
				{
					gameObject = wrapper.Prefab;
					return true;
				}
			}
			foreach(PrefabBundle bundle in  _bundles)
			{
				if(bundle.TryGet(name, out gameObject))
				{
					return true;
				}
			}
			gameObject = null;
			return false;
		}

		private bool TryRetrieve<T>(string name, out T component) where T : Component
		{
			if(TryRetrieve(name, out GameObject obj) && obj.TryGetComponent<T>(out component))
			{
				return true;
			}
			component = null;
			return false;
		}

  }

}