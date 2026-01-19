

namespace WrathTools
{
  public interface IAsyncAutoSaveProvider<TSelf> : IAutoSaveProvider<TSelf>, IAsyncSaveProvider<AutoSaveObject<TSelf>, TSelf>
    where TSelf : class, IAsyncAutoSaveProvider<TSelf>
  {

  }
}
