

namespace WrathTools
{
  public interface ISaveProvider<TSave> where TSave : SaveObject
  {

    TSave BuildSave();

  }
}
