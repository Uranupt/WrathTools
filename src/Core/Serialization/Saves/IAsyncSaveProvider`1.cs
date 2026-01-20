using System;
using System.Threading.Tasks;


namespace WrathTools
{

  public interface IAsyncSaveProvider<TSave> : ISaveProvider<TSave> where TSave : SaveObject
  {
    public Task<TSave> BuildSaveAsync(Action<TSave> onDone = null);

  }
}