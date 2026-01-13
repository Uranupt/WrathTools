using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace WrathTools.Unity
{
  public class ColliderComposite : MonoBehaviour
  {

    private static readonly Collider[] _colliderHits = new Collider[32];

    [SerializeField] private List<Collider> _colliders = new();
    private readonly List<Vector3> _colliderOffsets = new();
    private readonly List<Quaternion> _colliderRotations = new();

    public Collider[] GetColliderArray() => _colliders.ToArray();

    public bool CollisionCheck(Vector3 position, Quaternion rotation, LayerMask layers)
    {
      if(!ValidityCheck())
      {
        return true;
      }
      int hits = OverlapBoxFromCollectiveBounds(position, rotation, layers);
      return PenetrationTest(position, rotation, hits, 0f);
    }

    public bool CollisionCheck(Vector3 position, Quaternion rotation, LayerMask intolerantMask,
      LayerMask tolerantMask, float tolerance = 0.1f)
    {
      if(CollisionCheck(position, rotation, intolerantMask))
      {
        return true;
      }
      int hits = OverlapBoxFromCollectiveBounds(position, rotation, tolerantMask);
      return PenetrationTest(position, rotation, hits, tolerance);
    }

    public bool CollisionCheckFromPrefab(Vector3 scale, Vector3 position, Quaternion rotation,
      LayerMask layers, params Collider[] toIgnore)
    {
      if(!ValidityCheck(prefab: true))
      {
        return true;
      }
      return false;
    }

    public bool CollisionCheckFromPrefab(Vector3 scale, Vector3 position, Quaternion rotation,
      LayerMask intolerantMask, LayerMask tolerantMask, float tolerance, params Collider[] toIgnore)
    {
      if(CollisionCheckFromPrefab(scale, position, rotation, intolerantMask, toIgnore))
      {
        return true;
      }
      return false;
    }

    private int OverlapBoxFromCollectiveBounds(Vector3 position, Quaternion rotation, LayerMask layers)
    {
      Bounds bounds = new(transform.InverseTransformPoint(_colliders[0].bounds.center), _colliders[0].bounds.size);
      for(int i = 1; i < _colliders.Count; i++)
      {
        Bounds local = new(transform.InverseTransformPoint(_colliders[i].bounds.center), _colliders[i].bounds.size);
        bounds.Encapsulate(local.min);
        bounds.Encapsulate(local.max);
      }
      return Physics.OverlapBoxNonAlloc(
        position + (rotation * bounds.center),
        bounds.extents,
        _colliderHits,
        rotation,
        layers
      );
    }

    private bool ValidityCheck(bool prefab = false)
    {
      if(prefab && !gameObject.IsPrefab())
      {
        Debug.LogError("Cannot perform prefab collision check on an instantiated object. Returning unconditional true.");
        return false;
      }
      if(_colliders.Count == 0)
      {
        Debug.LogError("Cannot perform collision check on a ColliderComposite with no Colliders.");
        return false;
      }
      return true;
    }

    private bool PenetrationTest(Vector3 position, Quaternion rotation, int hits, float tolerance, params Collider[] toIgnore)
    {
      //TODO: Split logic when Collider count exceeds some value
      if(hits <= 0)
      {
        return false;
      }
      for(int i = 0; i < hits; i++)
      {
        Collider hitCollider = _colliderHits[i];
        if(toIgnore.Contains(hitCollider) || _colliders.Contains(hitCollider)) { continue; }
        for(int k = 0; k < _colliders.Count; k++)
        {
          Quaternion worldRotation = rotation * _colliderRotations[k];
          if(Physics.ComputePenetration(
            _colliders[k],
            position + (worldRotation * _colliderOffsets[k]),
            worldRotation,
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
      }
      return false;
    }

    private void UpdateOffsetsAndRotations()
    {
      for(int i = 0; i < _colliders.Count; i++)
      {
        _colliderOffsets[i] = transform.InverseTransformPoint(_colliders[i].transform.position);
        _colliderRotations[i] = Quaternion.Inverse(transform.rotation) * _colliders[i].transform.rotation;
      }
    }

  }
}