using System;
using System.Threading.Tasks;


namespace WrathTools.Deprecated
{
  [Obsolete("SaveBridge has been deprecated in favor of BinarySerialization")]
  public interface IAsyncSaveProvider<TSave> : ISaveProvider<TSave> where TSave : SaveObject
  {
    public Task<TSave> BuildSaveAsync(Action<TSave> onDone = null);

  }
}