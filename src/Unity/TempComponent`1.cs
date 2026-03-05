using System;
using UnityEngine;


namespace WrathTools.Unity
{
  public class TempComponent<T> : TempGameObject where T : Component
  {

    private T _component;

    public T Component
    {
      get
      {
        if(_component != null && this.GameObject == null)
        {
          _component = null;
        }
        return _component;
      }
    }

    public override bool IsValid => base.IsValid && Component != null;

    public TempComponent(T component) : base(component != null ? component.gameObject : null)
    {
      if(this.GameObject == null) { return; }
      _component = this.GameObject.GetComponent<T>();
    }

    public bool TryInstantiate(out T resl, Transform parent = null)
    {
      if(IsValid)
      {
        resl = GameObject.Instantiate(Component, parent);
        return true;
      }
      resl = null;
      return false;
    }

    public new T Instantiate(Transform parent = null)
    {
      if(!TryInstantiate(out T resl, parent))
      {
        UnityDiagnostics.LogError(
          new Exception("Cannot create an instance of a null, destroyed, or moved GameObject"),
          stackTrace: new(true),
          id: UnityDiagnostics.DiagnosticID + ".temp_objects.invalid_target.component"
        );
      }
      return resl;
    }

    public GameObject InstantiateGameObject(Transform parent = null) => base.Instantiate(parent);

  }
}