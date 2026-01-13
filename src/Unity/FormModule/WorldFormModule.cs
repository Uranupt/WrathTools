using System;
using UnityEngine;


namespace WrathTools.Unity
{
  public sealed class WorldFormModule
  {
    private Collider _collider;
    private ColliderComposite _composite;
    private MeshFilter _filter;
    private MeshRenderer _renderer;

    public MonoBehaviour Parent { get; private set; }
    public WorldFormInfo FormInfo { get; private set; }
    public Vector3 Position => Parent.transform.position;
    public Vector3 WorldScale => Parent.transform.lossyScale;
    public Quaternion Rotation => Parent.transform.rotation;

    public WorldFormModule(MonoBehaviour parent)
    {
      Parent = parent;
      InitMeshComponents();
    }

    public WorldFormModule(MonoBehaviour parent, WorldFormInfo formInfo)
    {
      Parent = parent;
      InitMeshComponents();
      SetFormInfo(formInfo);
    }

    public bool CollisionCheck(Vector3 position, LayerMask layers) => CollisionCheck(position, Parent.transform.rotation, layers);

    public bool CollisionCheck(Vector3 position, LayerMask intolerantMask, LayerMask tolerantMask, float tolerance = 0.1f)
    {
      return CollisionCheck(position, Parent.transform.rotation, intolerantMask, tolerantMask, tolerance);
    }

    public bool CollisionCheck(Vector3 position, Quaternion rotation, LayerMask layers)
    {
      if(FormInfo == default)
      {
        throw new InvalidOperationException("Cannot do a collision check without assigned WorldFormInfo.");
      }
      if(FormInfo.ColliderType == ColliderType.Composite)
      {
        return _composite.CollisionCheck(position, rotation, layers);
      }
      else
      {
        return PhysicsTools.CheckCollision(_collider, position, rotation, layers);
      }
    }

    public bool CollisionCheck(Vector3 position, Quaternion rotation, LayerMask intolerantMask,
      LayerMask tolerantMask, float tolerance = 0.1f)
    {
      if(FormInfo == default)
      {
        throw new InvalidOperationException("Cannot do a collision check without assigned WorldFormInfo.");
      }
      if(FormInfo.ColliderType == ColliderType.Composite)
      {
        return _composite.CollisionCheck(position, rotation, intolerantMask, tolerantMask, tolerance);
      }
      else
      {
        return PhysicsTools.CheckCollision(_collider, position, rotation, intolerantMask, tolerantMask, tolerance);
      }
    }

    public bool CollisionCheckWithForm(WorldFormInfo newForm, LayerMask layers)
    {
      Mesh mesh = newForm.ColliderType == ColliderType.ProxyMesh
        ? newForm.ProxyMesh
        : newForm.RenderMesh;
      Collider[] selfColliders = FormInfo.ColliderType == ColliderType.Composite
        ? _composite.GetColliderArray()
        : new Collider[1]{ _collider };
      switch(newForm.ColliderType)
      {
        case ColliderType.RenderMesh:
        case ColliderType.ProxyMesh:
        {
          return PhysicsTools.CheckMesh(mesh, WorldScale, Position, Rotation, layers, selfColliders);
        }
        case ColliderType.CapsuleFromBounds:
        {
          return PhysicsTools.CheckCapsuleFromBounds(mesh.bounds, WorldScale, Position, Rotation, layers, 0f, selfColliders);
        }
        case ColliderType.BoxFromBounds:
        {
          return PhysicsTools.CheckBoxFromBounds(mesh.bounds, WorldScale, Position, Rotation, layers, 0f, selfColliders);
        }
        case ColliderType.Composite:
        {
          return newForm.CompositePrefab.CollisionCheckFromPrefab(WorldScale, Position, Rotation, layers, selfColliders);
        }
        default:
        {
          Debug.LogError("Invalid ColliderType");
          return true;
        }
      }
    }

    public bool CollisionCheckWithForm(WorldFormInfo newForm, LayerMask intolerantMask, LayerMask tolerantMask, float tolerance = 0.1f)
    {
      Mesh mesh = newForm.ColliderType == ColliderType.ProxyMesh
      ? newForm.ProxyMesh
      : newForm.RenderMesh;
      Collider[] selfColliders = FormInfo.ColliderType == ColliderType.Composite
        ? _composite.GetColliderArray()
        : new Collider[1] { _collider };
      switch(newForm.ColliderType)
      {
        case ColliderType.RenderMesh:
        case ColliderType.ProxyMesh:
        {
          return PhysicsTools.CheckMesh(mesh, WorldScale, Position, Rotation, intolerantMask, tolerantMask, tolerance, selfColliders);
        }
        case ColliderType.CapsuleFromBounds:
        {
          return PhysicsTools.CheckCapsuleFromBounds(mesh.bounds, WorldScale, Position, Rotation, intolerantMask, 0f, selfColliders)
            || PhysicsTools.CheckCapsuleFromBounds(mesh.bounds, WorldScale, Position, Rotation, tolerantMask, tolerance, selfColliders);
        }
        case ColliderType.BoxFromBounds:
        {
          return PhysicsTools.CheckBoxFromBounds(mesh.bounds, WorldScale, Position, Rotation, intolerantMask, 0f, selfColliders)
            || PhysicsTools.CheckBoxFromBounds(mesh.bounds, WorldScale, Position, Rotation, tolerantMask, tolerance, selfColliders);
        }
        case ColliderType.Composite:
        {
          return newForm.CompositePrefab.CollisionCheckFromPrefab(WorldScale, Position, Rotation, intolerantMask, 
            tolerantMask, tolerance, selfColliders);
        }
        default:
        {
          Debug.LogError("Invalid ColliderType");
          return true;
        }
      }
    }

    public void SetFormInfo(WorldFormInfo formInfo)
    {
      FormInfo = formInfo;
      ClearCollider();
      BuildCollider();
      _filter.sharedMesh = FormInfo.RenderMesh;
      _renderer.sharedMaterial = FormInfo.Material;
    }

    private void InitMeshComponents()
    {
      Parent.gameObject.Lazy(ref _filter);
      Parent.gameObject.Lazy(ref _renderer);
      if(_filter == null)
      {
        _filter = Parent.gameObject.AddComponent<MeshFilter>();
      }
      if(_renderer == null)
      {
        _renderer = Parent.gameObject.AddComponent<MeshRenderer>();
      }
    }

    private void ClearCollider()
    {
      if(_collider != null)
      {
        GameObject.Destroy(_collider);
        _collider = null;
      }

      if(_composite != null)
      {
        GameObject.Destroy(_composite.gameObject);
        _composite = null;
      }
    }

    private void BuildCollider()
    {

      switch(FormInfo.ColliderType)
      {
        case ColliderType.RenderMesh:
        {
          _collider = Parent.gameObject.AddComponent<MeshCollider>();
          (_collider as MeshCollider).sharedMesh = FormInfo.RenderMesh;
          break;
        }
        case ColliderType.ProxyMesh:
        {
          _collider = Parent.gameObject.AddComponent<MeshCollider>();
          (_collider as MeshCollider).sharedMesh = FormInfo.ProxyMesh;
          break;
        }
        case ColliderType.CapsuleFromBounds:
        {
          CapsuleCollider capsule = Parent.gameObject.AddComponent<CapsuleCollider>();
          Bounds bounds = FormInfo.RenderMesh.bounds;
          Vector3 scaledBounds = Vector3.Scale(bounds.size, WorldScale);
          capsule.height = scaledBounds.y;
          capsule.radius = Mathf.Min(scaledBounds.x, scaledBounds.z);
          capsule.center = Rotation * bounds.center;
          _collider = capsule;
          break;
        }
        case ColliderType.BoxFromBounds:
        {
          BoxCollider box = Parent.gameObject.AddComponent<BoxCollider>();
          Bounds bounds = FormInfo.RenderMesh.bounds;
          box.center = Rotation * bounds.center;
          box.size = Vector3.Scale(bounds.size, WorldScale);
          _collider = box;
          break;
        }
        case ColliderType.Composite:
        {
          _composite = GameObject.Instantiate(FormInfo.CompositePrefab, Parent.transform, false);
          _composite.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
          break;
        }
      }

    }

    private bool HasColliderDiff(WorldFormInfo newForm)
    {
      if(FormInfo.ColliderType != newForm.ColliderType)
      {
        return true;
      }
      return FormInfo.ColliderType switch
      {
        ColliderType.RenderMesh or
        ColliderType.CapsuleFromBounds or
        ColliderType.BoxFromBounds => FormInfo.RenderMesh != newForm.RenderMesh,
        ColliderType.ProxyMesh => FormInfo.ProxyMesh != newForm.ProxyMesh,
        ColliderType.Composite => FormInfo.CompositePrefab != newForm.CompositePrefab,
        _ => true
      };
    }

  }
}