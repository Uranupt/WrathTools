using System;
using System.Threading.Tasks;


namespace WrathTools
{

  public interface IAsyncSaveProvider<TSave, TProvider> : ISaveProvider<TSave, TProvider>
    where TSave : SaveObject<TSave, TProvider>
    where TProvider : class, IAsyncSaveProvider<TSave, TProvider>
  {
    public Task<TSave> BuildSaveAsync(Action<TSave> onDone = null);

  }
}