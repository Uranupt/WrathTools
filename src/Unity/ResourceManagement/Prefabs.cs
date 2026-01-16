using UnityEngine;
using System.Collections.Generic;
using System;

namespace WrathTools.Unity.ResourceManagement
{
  public class Prefabs : SettingsObject<Prefabs>
  {

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

		public static bool TryGet<T>(string name, out T component) where T : Component
		{
			if(Instance.TryRetrieve(name, out T comp))
			{
				component = GameObject.Instantiate(comp);
				return true;
			}
			component = null;
			return false;
		}

		public static bool TryGetTemp<T>(string name, out TempComponent<T> tempComponent) where T : Component
		{
			if(Instance.TryRetrieve(name, out T comp))
			{
				tempComponent = new TempComponent<T>(comp);
				return true;
			}
			tempComponent = null;
			return false;
		}

		public static GameObject Get(string name)
		{
			if(TryGet(name, out GameObject obj))
			{
				return obj;
			}
			DiagnosticContext error = new UnityErrorContext(new Exception($"No Prefab found with name '{name}'"));
			return Diagnostics.ThrowOrDefault<GameObject>(error);
		}

		public static TempGameObject GetTemp(string name)
		{
			if(TryGetTemp(name, out TempGameObject obj))
			{
				return obj;
			}
      DiagnosticContext error = new UnityErrorContext(new Exception($"No Prefab found with name '{name}'"));
      return Diagnostics.ThrowOrDefault<TempGameObject>(error);
    }

		public static T Get<T>(string name) where T : Component
		{
			if(TryGet(name, out T comp))
			{
				return comp;
			}
			DiagnosticContext error = new UnityErrorContext(new Exception($"No Prefab found with name '{name}' and a Component of Type '{typeof(T).Name}'"));
			return Diagnostics.ThrowOrDefault<T>(error);
		}

		public static TempComponent<T> GetTemp<T>(string name) where T : Component
		{
			if(TryGetTemp(name, out TempComponent<T> comp))
			{
				return comp;
			}
      DiagnosticContext error = new UnityErrorContext(new Exception($"No Prefab found with name '{name}' and a Component of Type '{typeof(T).Name}'"));
      return Diagnostics.ThrowOrDefault<TempComponent<T>>(error);
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