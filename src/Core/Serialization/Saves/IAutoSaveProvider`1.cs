

namespace WrathTools
{ 
  public interface IAutoSaveProvider<TSelf> : IAutoSaveProvider, ISaveProvider<AutoSaveObject<TSelf>>
    where TSelf : class, IAutoSaveProvider<TSelf>
  {

  }
}
