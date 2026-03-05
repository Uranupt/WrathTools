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
      ColliderComposite composite = null
    )
    {
      RenderMesh = render;
      this.Material = material;
      this.ColliderType = colliderType;
      ProxyMesh = proxy;
      CompositePrefab = composite;
      Validate();
    }

    public WorldFormInfo WithCollider(ColliderType colliderType, Mesh proxy = null, ColliderComposite composite = null)
    {
      return new WorldFormInfo(
        RenderMesh,
        this.Material,
        colliderType,
        proxy,
        composite
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
        UnityDiagnostics.LogWarning(
          "Proxy Mesh Collider type selected, but no proxy mesh provided. Setting type to RenderMesh instead.",
          id: WorldForm.DiagnosticID + ".missing_proxy_mesh"
        );
        this.ColliderType = ColliderType.RenderMesh;
      }
      if(this.ColliderType == ColliderType.Composite && CompositePrefab == null)
      {

        UnityDiagnostics.LogWarning(
          "Composite Collider type selected, but no Composite prefab provided. Setting type to RenderMesh instead.",
          id: WorldForm.DiagnosticID + ".missing_composite_mesh"
        );
        this.ColliderType = ColliderType.RenderMesh;
      }
    }

  }
}