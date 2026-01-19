

namespace WrathTools
{

  public interface ISaveProvider<TSave, TProvider> 
    where TSave : SaveObject<TSave, TProvider>
    where TProvider : class, ISaveProvider<TSave, TProvider>
  {
    public TSave BuildSave();
  }
}