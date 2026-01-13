using System.Linq;
using UnityEngine;


namespace WrathTools.Unity
{
  public static class PhysicsTools
  {

    private static readonly Collider[] _colliderHits = new Collider[32];

    public static bool CheckCollision(Collider collider, Vector3 position, Quaternion rotation, LayerMask mask,
      params Collider[] toIgnore)
    {
      int hits = Physics.OverlapBoxNonAlloc(
        GetWithColliderOffset(position, collider),
        collider.UnrotatedExtents(),
        _colliderHits,
        rotation,
        mask
      );
      return PenetrationTest(collider, position, rotation, hits, toIgnore);
    }

    public static bool CheckCollision(Collider collider, Vector3 position, Quaternion rotation,
      LayerMask intolerantMask, LayerMask tolerantMask, float tolerance, params Collider[] toIgnore)
    {
      if(CheckCollision(collider, position, rotation, intolerantMask))
      {
        return true;
      }
      int hits = Physics.OverlapBoxNonAlloc(
        GetWithColliderOffset(position, collider),
        collider.UnrotatedExtents(),
        _colliderHits,
        rotation,
        tolerantMask
      );
      return PenetrationTest(collider, position, rotation, hits, toIgnore, tolerance);
    }

    public static bool CheckPenetration(Vector3 point, LayerMask layers)
    {
      return Physics.OverlapSphereNonAlloc(point, 0.05f, _colliderHits, layers) > 0;
    }

    public static bool CheckCapsuleFromBounds(Bounds bounds, Vector3 scale, Vector3 position, Quaternion rotation, 
      LayerMask layers, float tolerance, params Collider[] toIgnore)
    {
      float height = bounds.size.y * scale.y;
      float radius = Mathf.Min(bounds.size.x * scale.x, bounds.size.z * scale.z) - tolerance;
      radius = Mathf.Max(radius, 0f);
      Vector3 center = position + (rotation * bounds.center);
      Vector3 p0 = center + (rotation * Vector3.up * ((height / 2f) - radius));
      Vector3 p1 = center - (rotation * Vector3.up * ((height / 2f) - radius));
      int hits = Physics.OverlapCapsuleNonAlloc(p0, p1, radius, _colliderHits, layers);
      return TestHits(hits, toIgnore);
    }
    public static bool CheckBoxFromBounds(Bounds bounds, Vector3 scale, Vector3 position, Quaternion rotation,
      LayerMask layers, float tolerance, params Collider[] toIgnore)
    {
      Vector3 extents = (Vector3.Scale(bounds.size, scale) * 0.5f) - Vector3.one * tolerance;
      extents = Vector3.Max(extents, Vector3.zero);
      Vector3 center = position + (rotation * bounds.center);
      int hits = Physics.OverlapBoxNonAlloc(center, extents, _colliderHits, rotation, layers);
      return TestHits(hits, toIgnore);
    }

    public static bool CheckMesh(Mesh mesh, Vector3 scale, Vector3 position, Quaternion rotation,
      LayerMask layers, params Collider[] toIgnore)
    {
      MeshCollider collider = BuildTempCollider(mesh, scale, position, rotation);
      bool hit = CheckCollision(collider, position, rotation, layers, toIgnore);
      GameObject.DestroyImmediate(collider.gameObject);
      return hit;
    }

    public static bool CheckMesh(Mesh mesh, Vector3 scale, Vector3 position, Quaternion rotation,
      LayerMask intolerantMask, LayerMask tolerantMask, float tolerance, params Collider[] toIgnore)
    {
      MeshCollider collider = BuildTempCollider(mesh, scale, position, rotation);
      bool hit = CheckCollision(collider, position, rotation, intolerantMask, tolerantMask, tolerance, toIgnore);
      GameObject.DestroyImmediate(collider.gameObject);
      return hit;
    }

    public static Vector3 UnrotatedExtents(this Collider collider)
    {
      return collider switch
      {
        BoxCollider box => box.size * 0.5f,
        CapsuleCollider cap => new Vector3(cap.radius, cap.height * 0.5f, cap.radius),
        SphereCollider sphere => Vector3.one * sphere.radius,
        MeshCollider mesh => mesh.sharedMesh.bounds.extents,
        _ => collider.bounds.extents
      };
    }

    private static Vector3 GetWithColliderOffset(Vector3 pos, Collider collider) => pos + (collider.transform.position - collider.bounds.center);

    private static MeshCollider BuildTempCollider(Mesh mesh, Vector3 scale, Vector3 position, Quaternion rotation)
    {
      GameObject tempObj = new("MeshCheck");
      tempObj.hideFlags = HideFlags.HideAndDontSave;
      tempObj.transform.SetPositionAndRotation(position, rotation);
      tempObj.transform.localScale = scale;
      MeshCollider collider = tempObj.AddComponent<MeshCollider>();
      collider.sharedMesh = mesh;
      collider.convex = false;
      return collider;
    }

    private static bool TestHits(int hits, Collider[] toIgnore)
    {
      if(hits == 0)
      {
        return false;
      }
      for(int i = 0; i < hits; i++)
      {
        if(!toIgnore.Contains(_colliderHits[i]))
        {
          return true;
        }
      }
      return false;
    }

    private static bool PenetrationTest(Collider collider, Vector3 position, Quaternion rotation, int hits, 
      Collider[] toIgnore, float tolerance = 0f)
    {
      if(hits <= 0)
      {
        return false;
      }
      for(int i = 0; i < hits; i++)
      {
        Collider hitCollider = _colliderHits[i];
        if(hitCollider == collider || toIgnore.Contains(hitCollider)) { continue; }
        if(Physics.ComputePenetration(
          collider,
          position,
          rotation,
          hitCollider,
          hitCollider.transform.position,
          hitCollider.transform.rotation,
          out Vector3 _,
          out float distance
        ))
        {
          if(distance > tolerance)
          {
            return true;
          }
        }
      }
      return false;
    }

  }
}