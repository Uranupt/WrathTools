using System;
using UnityEngine;


namespace WrathTools.Unity
{
  public class TempGameObject : IDisposable
  {

    private static GameObject _container;

    protected static GameObject Container
    {
      get
      {
        if(_container == null)
        {
          _container = new GameObject("TempGameObject Container");
          _container.hideFlags = HideFlags.HideAndDontSave;
          _container.SetActive(false);
        }
        return _container;
      }
    }

    private GameObject _gameObject;

    public GameObject GameObject
    {
      get
      {
        if(_gameObject != null && _gameObject.transform.parent != Container.transform)
        {
          _gameObject = null;
        }
        return _gameObject;
      }
    }

    public virtual bool IsValid => GameObject != null;

    public TempGameObject(GameObject obj)
    {
      if(obj == null) { return; }
      _gameObject = GameObject.Instantiate(obj, Container.transform);
    }

    public void Destroy()
    {
      if(!IsValid) { return; }
      GameObject.Destroy(_gameObject);
    }

    public void Dispose() => Destroy(); 

    public bool TryInstantiate(out GameObject resl, Transform parent = null)
    {
      if(IsValid)
      {
        resl = GameObject.Instantiate(_gameObject, parent);
        return true;
      }
      resl = null;
      return false;
    }

    public GameObject Instantiate(Transform parent = null)
    {
      if(!TryInstantiate(out GameObject resl, parent))
      {
        UnityDiagnostics.LogError(
          new Exception("Cannot create an instance of a null, destroyed, or moved GameObject"),
          stackTrace: new(true)
        );
      }
      return resl;
    }

  }
}
