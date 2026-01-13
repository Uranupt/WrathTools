using UnityEngine;
using System;

namespace WrathTools.Unity.ResourceManagement
{
	[Serializable]
	internal class PrefabWrapper
	{

		[SerializeField, ReadOnly] public string Name;
		public GameObject Prefab;

	}
}