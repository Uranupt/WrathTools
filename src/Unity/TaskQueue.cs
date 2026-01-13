using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace WrathTools.Unity
{
  public sealed class TaskQueue
  {

    private readonly Queue<Func<Task>> _queue = new();
    private bool _running;

    public void Enqueue(Func<Task> task)
    {
      if(task == null) { return; }
      _queue.Enqueue(task);
    }

    public void Clear() => _queue.Clear();

    public async void Run(Action onDone = null)
    {
      if(_running) { return; }
      await RunAwaitable();
      onDone?.Invoke();
    }

    private async Task RunAwaitable()
    {
      _running = true;
      while(_queue.Count > 0)
      {
        Func<Task> task = _queue.Dequeue();
        try
        {
          await task.Invoke();
        }
        catch(Exception e)
        {
          Debug.LogException(e);
        }
      }
    }

  }
}