using System.Collections.Generic;
using System.Collections;
using UnityEngine;


namespace WrathTools.Unity
{
  public sealed class FrameScheduler : MonoBehaviour
  {

    public interface IJob
    {
      public bool DoWork();
      public void FinishWork();
    }

    private static FrameScheduler _instance;
    private static FrameScheduler Instance
    {
      get
      {
        if(_instance == null)
        {
          _instance = new GameObject().AddComponent<FrameScheduler>();
          DontDestroyOnLoad(_instance.gameObject);
        }
        return _instance;
      }
    }

    private float _startTime;
    private bool _running = false;
    private readonly List<IJob> _jobs = new();

    public static void Schedule(IJob job) => Instance.SchedulePrivate(job);

    private void Awake()
    {
      if(_instance == null)
      {
        _instance = this;
        DontDestroyOnLoad(gameObject);
      }
      else if(_instance != null)
      {
        Destroy(gameObject);
      }
    }

    private void Update()
    {
      _startTime = Time.realtimeSinceStartup;
    }

    private void SchedulePrivate(IJob job)
    {
      if(_jobs.Contains(job)) { return; }
      _jobs.Add(job);
      if(!_running)
      {
        StartCoroutine(Run());
      }
    }

    private IEnumerator Run()
    {
      _running = true;
      while(_jobs.Count > 0)
      {
        for(int i = 0; i < _jobs.Count; i++)
        {
          if(!_jobs[i].DoWork())
          {
            _jobs[i].FinishWork();
            _jobs.Remove(_jobs[i]);
            i--;
          }
          if(Time.realtimeSinceStartup - _startTime > 0.016f)
          {
            yield return null;
          }
        }
      }
      _running = false;
      yield return null;
    }

  }
}