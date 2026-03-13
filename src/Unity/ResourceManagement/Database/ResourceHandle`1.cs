using System;
using UnityEngine;


namespace WrathTools.Unity.ResourceManagement
{
  public sealed class ResourceHandle<T> : ResourceHandle where T : class, IResourceObject
  {

    public ResourceHandle(int id, bool exactType = true) 
      : base(id, resourceType : typeof(T), exactResourceType : exactType)
    {

    }

    public new T Resource => base.Resource as T;

  }
}