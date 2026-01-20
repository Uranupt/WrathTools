using System.Collections.Generic;


namespace WrathTools
{
  public class SaveCollection<TSave, TLoad> : SaveObject<IEnumerable<TLoad>> where TSave : SaveObject<TLoad>
  { 



  }
}