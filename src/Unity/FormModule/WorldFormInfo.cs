using System;
using UnityEngine;

namespace WrathTools.Unity
{
  [Serializable]
  public class WorldFormInfo
  {
    [field: SerializeField] public Mesh RenderMesh { get; private set; }
    [field: SerializeField] public Material Material { get; private set; }
    [field: SerializeField] public ColliderType ColliderType { get; private set; }
    [field: SerializeField] public Mesh ProxyMesh { get; private set; }
    [field: SerializeField] public ColliderComposite CompositePrefab { get; private set; }

    public WorldFormInfo(
      Mesh render,
      Material material,
      ColliderType colliderType,
      Mesh proxy = null,
      ColliderComposite amalgam = null
    )
    {
      RenderMesh = render;
      this.Material = material;
      this.ColliderType = colliderType;
      ProxyMesh = proxy;
      CompositePrefab = amalgam;
      Validate();
    }

    public WorldFormInfo WithCollider(ColliderType colliderType, Mesh proxy = null, ColliderComposite amalgam = null)
    {
      return new WorldFormInfo(
        RenderMesh,
        this.Material,
        colliderType,
        proxy,
        amalgam
      );
    }

    public WorldFormInfo WithMesh(Mesh mesh)
    {
      return new WorldFormInfo(
        mesh,
        this.Material,
        this.ColliderType,
        ProxyMesh,
        CompositePrefab
      );
    }

    public WorldFormInfo WithMaterial(Material material)
    {
      return new WorldFormInfo(
        RenderMesh,
        material,
        this.ColliderType,
        ProxyMesh,
        CompositePrefab
      );
    }

    public void Validate()
    {
      if(this.ColliderType == ColliderType.ProxyMesh && ProxyMesh == null)
      {
        Debug.LogWarning("Proxy Mesh Collider type selected, but no proxy mesh provided. Setting type to RenderMesh instead.");
        this.ColliderType = ColliderType.RenderMesh;
      }
      if(this.ColliderType == ColliderType.Composite && CompositePrefab == null)
      {
        Debug.LogWarning("Primitive Amalgam Collider type selected, but no amalgam prefab provided. Setting type to RenderMesh instead.");
        this.ColliderType = ColliderType.RenderMesh;
      }
    }

  }
}