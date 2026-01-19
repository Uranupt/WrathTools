using System.Collections.Generic;


namespace WrathTools
{
  public interface ISaveObjectCollection<TSave>
  {
    IReadOnlyCollection<TSave> Collection { get; }
    void Add(TSave save);
  }
}