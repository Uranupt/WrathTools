using UnityEngine;
using System;


namespace WrathTools.Unity.ResourceManagement
{
  public abstract class ResourceObject : ScriptableObject
  {

    public static event Action<ResourceObject> ResourceIDChanged;
    public static event Action<ResourceObject> ResourceValidated;

    [field: SerializeField, ReadOnly] public int ID { get; private set; } = -1;
    public abstract Type BuildType { get; }
    public virtual bool AutomaticallyUpdatePeek => true;

    public abstract bool TryBuild<T>(out T resl, bool exactType = true) where T : class;
    public abstract T Build<T>(bool exactType = true) where T : class;

    public abstract ResourcePeek GetPeek();

    protected virtual void OnIDChanged(int old)
    {

    }

    internal void SetID(int id)
    {
      int old = ID;
      ID = id;
      ResourceIDChanged?.Invoke(this);
      OnIDChanged(old);
    }

    private void OnValidate()
    {
      if(AutomaticallyUpdatePeek)
      {
        ResourceValidated?.Invoke(this);
      }
    }

  }
}