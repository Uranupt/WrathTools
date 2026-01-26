using System;
using System.Threading.Tasks;


namespace WrathTools.Deprecated
{
  [Obsolete("SaveBridge has been deprecated in favor of BinarySerialization")]
  public abstract class SaveObject<TProvider> : SaveObject
  { 

    public sealed override Type LoadType => typeof(TProvider);

    public TProvider Load()
    {
      if(!Valid)
      {
        Diagnostics.LogError(
          new InvalidOperationException($"SaveObjects must be Valid to be used. Build State Flags: {State}"),
          stackTrace: new(true)
        );
        return default;
      }
      TProvider resl = LoadProtected();
      MarkConsumed();
      return resl;
    }

    public async Task<TProvider> LoadAsync(Action<TProvider> onDone = null)
    {
      if(!Valid)
      {
        Diagnostics.LogError(
          new InvalidOperationException($"SaveObjects must be Valid to be used. Build State Flags: {State}"),
          stackTrace: new(true)
        );
        onDone?.Invoke(default);
        return default;
      }
      TProvider resl = await LoadAsyncProtected();
      MarkConsumed();
      onDone?.Invoke(resl);
      return resl;
    }

    protected abstract TProvider LoadProtected();
    protected abstract Task<TProvider> LoadAsyncProtected();

    internal sealed override T LoadInternal<T>()
    {
      return LoadProtected() as T;
    }

    internal async sealed override Task<T> LoadAsyncInternal<T>()
    {
      return await LoadAsyncProtected() as T;
    }

  }
}