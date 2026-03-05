using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace WrathTools.Unity
{
  public class ColliderComposite : MonoBehaviour
  {

    private static readonly Collider[] _colliderHits = new Collider[32];

    [SerializeField, HideInInspector] private List<Collider> _colliders = new();
    private Vector3[] _colliderOffsets = new Vector3[0];
    private Quaternion[] _colliderRotations = new Quaternion[0];

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
      if(!ValidityCheck(true))
      {
        return true;
      }
      OnTransformChildrenChanged();
      int hits = OverlapBoxFromCollectiveBounds(position, rotation, layers);
      return PenetrationTest(position, rotation, hits, 0f, toIgnore);
    }

    public bool CollisionCheckFromPrefab(Vector3 scale, Vector3 position, Quaternion rotation,
      LayerMask intolerantMask, LayerMask tolerantMask, float tolerance, params Collider[] toIgnore)
    {
      if(CollisionCheckFromPrefab(scale, position, rotation, intolerantMask, toIgnore))
      {
        return true;
      }
      int hits = OverlapBoxFromCollectiveBounds(position, rotation, tolerantMask);
      return PenetrationTest(position, rotation, hits, tolerance, toIgnore);
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
        UnityDiagnostics.LogError(
          new InvalidOperationException("Cannot perform prefab collision check on an instantiated object. Returning unconditional true."),
          stackTrace: new(true),
          id: WorldForm.DiagnosticID + ".invalid_composite_check.prefab"
        );
        return false;
      }
      if(_colliders.Count == 0)
      {
        UnityDiagnostics.LogError(
          new InvalidOperationException("Cannot perform collision check on a ColliderComposite with no Colliders."),
          stackTrace: new(true),
          id: WorldForm.DiagnosticID + ".composite_missing_colliders"
        );
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
      _colliderOffsets = new Vector3[_colliders.Count];
      _colliderRotations = new Quaternion[_colliders.Count];
      for(int i = 0; i < _colliders.Count; i++)
      {
        _colliderOffsets[i] = transform.InverseTransformPoint(_colliders[i].transform.position);
        _colliderRotations[i] = Quaternion.Inverse(transform.rotation) * _colliders[i].transform.rotation;
      }
    }

    private void OnEnable()
    {
      OnTransformChildrenChanged();
    }

    private void OnTransformChildrenChanged()
    {
      _colliders = new();
      foreach(Transform child in transform)
      {
        if(child.TryGetComponent(out Collider collider))
        {
          _colliders.Add(collider);
        }
      }
      UpdateOffsetsAndRotations();
    }

    private void OnTransformParentChanged()
    {
      UpdateOffsetsAndRotations();
    }

  }
}