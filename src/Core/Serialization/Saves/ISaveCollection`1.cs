using System.Collections.Generic;


namespace WrathTools
{
  public interface ISaveCollection<TSave>
  {
    IReadOnlyCollection<TSave> Collection { get; }
    void Add(TSave save);
  }
}