using System;
using System.IO;
using System.Threading.Tasks;


namespace WrathTools
{
  public abstract class SaveObject<TSave, TProvider> : SaveObject
    where TSave : SaveObject<TSave, TProvider>
    where TProvider : class, ISaveProvider<TSave, TProvider>
  {

    public override Type Type => typeof(TProvider);

    public TProvider Load()
    {
      TProvider resl = LoadProtected();
      MarkConsumed();
      return resl;
    }

    public async Task<TProvider> LoadAsync(Action<TProvider> onDone = null)
    {
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