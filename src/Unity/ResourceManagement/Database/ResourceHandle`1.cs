using System;
using UnityEngine;


namespace WrathTools.Unity.ResourceManagement
{
  public sealed class ResourceHandle<T> : ResourceHandle where T : ResourceObject
  {

    public ResourceHandle(int id, bool exactType = true) 
      : base(id, resourceType : typeof(T), exactResourceType : exactType)
    {

    }

    public new T Resource => (T)base.Resource;

  }
}