using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.AI;
using System.IO;
using System.Linq;

namespace WrathTools.Unity
{
	public static class UnityTools
	{

		public static RaycastHit[] RaycastBuffer = new RaycastHit[200];
		public static Collider[] ColliderBuffer = new Collider[200];

		public static Vector2 Abs(this Vector2 vector)
		{
			return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
		}

		public static Vector3 Abs(this Vector3 vector)
		{
			return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
		}

		public static Vector4 Abs(this Vector4 vector)
		{
			return new Vector4(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z), Mathf.Abs(vector.w));
		}

		public static bool NullProp<T>(this T instance, Action<T> action) where T : MonoBehaviour
		{
			if(instance != null)
			{
				action.Invoke(instance);
				return true;
			}
			return false;
		}

		public static void SetLayerCascading(this GameObject gameObject, int layer)
		{
			gameObject.layer = layer;
			for(int i = 0; i < gameObject.transform.childCount; i++)
			{
				gameObject.transform.GetChild(i).gameObject.SetLayerCascading(layer);
			}
		}

		public static void SetLayerCascading(this GameObject gameObject, string layerName)
		{
			gameObject.SetLayerCascading(LayerMask.NameToLayer(layerName));
		}

		public static void SetLayerCascading(this GameObject gameObject, LayerMask mask)
		{
			gameObject.SetLayerCascading((int)mask);
		}

		public static int ToLayer(this LayerMask layerMask)
		{
			int resl = 0;
			int maskShift = (int)layerMask;
			while(maskShift > 1)
			{
				resl++;
				maskShift >>= 1;
			}
			return resl;
		}

		public static List<string> GetNames(this RenderingLayerMask mask)
		{
			string[] layerNames = RenderingLayerMask.GetDefinedRenderingLayerNames();
			List<string> resl = new();
			uint value = (uint)mask;
			for(int i = 0; i < layerNames.Length; i++)
			{
				if((value & 1u << i) != 0)
				{
					resl.Add(layerNames[i]);
				}
			}
			return resl;
		}


		/// <summary>
		/// Lazy helper for Components, with the ability to add the Component if it's missing.
		/// Must be a MonoBehaviour.
		/// Not recommended if Component requires Inspector assigned fields.
		/// </summary>
		public static T LazyOrBuild<T>(this GameObject obj, ref T field) where T : MonoBehaviour
		{
			if(field == null)
			{
				if(!obj.TryGetComponent(out field))
				{
					field = obj.AddComponent<T>();
				}
			}
			return field;
		}

		/// <summary>
		/// Lazy helper for Components. Will throw or log warning if it's missing, and return null if throwing is not allowed.
		/// </summary>
		public static T Lazy<T>(this GameObject obj, ref T field) where T : Component
		{
			if(field == null)
			{
				if(!obj.TryGetComponent(out field))
				{
					UnityDiagnostics.LogError(
						new Exception($"The provided GameObject was missing the expected Component of Type '{typeof(T).Name}'"),
						stackTrace: new(true)
					);
				}
			}
			return field;
		}

		public static T LazyFromScene<T>(ref T field, Func<T, bool> predicate = null) where T : Component
		{
			if(field == null)
			{
				T[] objects = UnityEngine.Object.FindObjectsByType<T>(FindObjectsSortMode.InstanceID);
				field = predicate != null ? objects.FirstOrDefault(predicate) : objects.FirstOrDefault();
				if(field == null)
				{
					UnityDiagnostics.LogError(
						new Exception($"No Component of  Type '{typeof(T).Name}' matching the provdid predicate was found in the Scene"),
						stackTrace: new(true)
					);
				}
			}
			return field;
		}

		public static Bounds CalculateNavMeshBounds(List<NavMeshBuildSource> sources)
		{
			if(sources.Count == 0)
			{
				return new Bounds(Vector3.zero, Vector3.one * 1f);
			}
			Mesh firstMesh = sources[0].sourceObject as Mesh;
			Matrix4x4 firstMatrix = sources[0].transform;
			Vector3 firstCenter = firstMesh ? firstMesh.bounds.center : Vector3.zero;
			Bounds resl = new(firstMatrix.MultiplyPoint(firstCenter), Vector3.zero);

			static float Calc(float x, float y) => Mathf.Abs(x) * y;

			foreach(NavMeshBuildSource source in sources)
			{
				if(source.sourceObject is not Mesh mesh) { continue; }
				Bounds local = mesh.bounds;
				Matrix4x4 matrix = source.transform;
				Vector3 worldCenter = matrix.MultiplyPoint(local.center);
				Vector3 x = new(matrix[0, 0], matrix[1, 0], matrix[2, 0]);
				Vector3 y = new(matrix[0, 1], matrix[1, 1], matrix[2, 1]);
				Vector3 z = new(matrix[0, 2], matrix[1, 2], matrix[2, 2]);
				Vector3 extents = local.extents;
				Vector3 worldExtents = new(
					Calc(x.x, extents.x) + Calc(y.x, extents.y) + Calc(z.x, extents.z),
					Calc(x.y, extents.x) + Calc(y.y, extents.y) + Calc(z.y, extents.z),
					Calc(x.z, extents.x) + Calc(y.z, extents.y) + Calc(z.z, extents.z)
				);
				Bounds worldBounds = new(worldCenter, worldExtents * 2f);
				resl.Encapsulate(worldBounds);
			}
			resl.Expand(5f);
			return resl;
		}

		public static string ToAssetPath(this string path)
		{
			return path.Replace('\\', '/').TrimEnd('/');
		}

		public static string GetParentPath(this string path)
		{
			return Path.GetDirectoryName(path).ToAssetPath();
		}

		public static bool IsPrefab(this GameObject obj)
		{
			if(obj == null)
			{
				return false;
			}
			return !obj.scene.IsValid() || !obj.scene.isLoaded;
		}

		public static Rect ScreenRectFromCenter(float width, float height, float xOffset = 0f, float yOffset = 0f)
		{
			return new Rect((Screen.width / 2) + xOffset, (Screen.height / 2) + yOffset, width, height);
		}

		/// <summary>
		/// Renders a Quaternion as a uint-stored aproximation of its component values. Precision beyond 3 decimal places will be lost, but is deterministic roundtrip on pack and unpack. 
		/// </summary>
    public static uint PackQuaternion(Quaternion value)
    {
      value = value.normalized;
      if(value.w < 0)
      {
        value = new Quaternion(-value.x, -value.y, -value.z, -value.w);
      }
      int maxIndex = 0;
      float maxValue = Mathf.Abs(value[0]);
      //Find highest value so it can be discarded
      for(int i = 1; i < 4; i++)
      {
        float component = Mathf.Abs(value[i]);
        if(component > maxValue)
        {
          maxValue = component;
          maxIndex = i;
        }
      }
      uint resl = (uint)maxIndex & 3; //Store index of dropped value

      //Store the remaining 3 raw to 10 bit (511) precision
      for(int i = 0, j = 2; i < 4; i++)
      {
        if(i == maxIndex) { continue; }
        float component = value[i] * Mathf.Sign(value[maxIndex]); //Preserves sign of droppped component
        resl |= ((uint)Mathf.RoundToInt((component + 1f) * 511f) << j);
        j += 10;
      }
      return resl;
    }

		/// <summary> Unpacks a Quaternion stored as a uint approximation via <see cref="PackQuaternion"/> </summary>
		public static Quaternion UnpackQuaternion(uint value)
    {
      int maxIndex = (int)(value & 3); //Retrieve index of dropped component
      int[] raw = new int[3];
      for(int i = 0, j = 2; i < 3; i++, j += 10)
      {
        raw[i] = (int)((value >> j) & 0x3ff); //Shift and trim by 0ing out bit outside component range, 0x3FF = 10 bits
      }
      float[] q = new float[4];
      q[maxIndex] = 0f;
      //Restore packed q by performing the inverse of Packing math
      for(int i = 0, j = 0; i < 4; i++)
      {
        if(i == maxIndex) { continue; }
        q[i] = (raw[j] / 511f) - 1f;
        q[maxIndex] += q[i] * q[i]; //Setting up dropped component reconstruction
        j++;
      }
      q[maxIndex] = Mathf.Sqrt(1f - q[maxIndex]);
      return new Quaternion(q[0], q[1], q[2], q[3]);
    }

  }
}