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

		}

    public GameObject Get(string name)
		{
			foreach(PrefabWrapper wrapper in _prefabs)
			{
				if(wrapper.Name == name)
				{
					return wrapper.Prefab;
				}
			}
			return null;
		}

    public T Get<T>(string name) where T : MonoBehaviour
		{
			foreach(PrefabWrapper wrapper in _prefabs)
			{
				if(wrapper.Name == name && wrapper.Prefab.TryGetComponent<T>(out T component))
				{
					return component;
				}
			}
			return null;
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