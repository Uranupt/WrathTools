using System.Collections.Generic;
using UnityEngine;

namespace WrathTools.Unity.ResourceManagement
{
	[CreateAssetMenu(menuName = "ResourceManagement/PrefabBundle")]
	public sealed class PrefabBundle : ScriptableObject
	{

		[field: SerializeField] public string Name { get; private set; }
		[SerializeField] private List<PrefabWrapper> _prefabs = new(); //Violating usual conventions for ordering in preference of Serialized display order

		public bool TryGet(string name, out GameObject gameObject)
		{
			foreach(PrefabWrapper wrapper in _prefabs)
			{
				if(wrapper.Name == name)
				{
					gameObject = wrapper.Prefab;
					return true;
				}
			}
			gameObject = null;
			return false;
		}

		public bool TryGet<T>(out T component, string name = null) where T : Component
		{
			name ??= typeof(T).Name;
			if(TryGet(name, out GameObject obj))
			{
				return obj.TryGetComponent(out component);
			}
			component = null;
			return false;
		}

		private void OnValidate()
		{
			foreach(PrefabWrapper wrapper in _prefabs)
			{
				wrapper.Name = wrapper.Prefab.name;
			}
		}


	}
}