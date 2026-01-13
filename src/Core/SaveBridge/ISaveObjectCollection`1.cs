using System.Collections.Generic;


namespace WrathTools
{
  /// <summary>
  /// Interface for collections of <see cref="SaveObject"/>s of Type <typeparamref name="TSave"/>.
  /// </summary>
  public interface ISaveObjectCollection<TSave>
  {
    /// <summary> Contained collection of <typeparamref name="TSave"/> instances. </summary>
    IReadOnlyCollection<TSave> Collection { get; }
    /// <summary> Adds a new <typeparamref name="TSave"/> instance to the collection. </summary>
    void Add(TSave save);
  }
}