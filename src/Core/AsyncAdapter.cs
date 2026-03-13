using System.Threading.Tasks;
using System;


namespace WrathTools
{
  public static class AsyncAdapter
  {

    public static async void ToCallback(Func<Task> func, Action onDone)
    {
      await func.Invoke();
      onDone?.Invoke();
    }

    public static async void ToCallback<T>(Func<Task<T>> func, Action<T> onDone)
    {
      onDone?.Invoke(await func.Invoke());
    }

    public static Task FromCallback(Action<Action> action)
    {
      TaskCompletionSource<object> taskSource = new();
      action.Invoke(() => taskSource.SetResult(null));
      return taskSource.Task;
    }

    public static Task<T> FromCallback<T>(Action<Action<T>> action)
    {
      TaskCompletionSource<T> taskSource = new();
      action.Invoke((v) => taskSource.SetResult(v));
      return taskSource.Task;
    }

  }
}