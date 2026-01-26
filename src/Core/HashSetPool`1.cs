using System.Collections.Generic;


namespace WrathTools
{
  public static class HashSetPool<T>
  {

    private static readonly Stack<HashSet<T>> _pool = new();

    public static HashSet<T> Get()
    {
      return _pool.Count > 0 ? _pool.Pop() : new HashSet<T>();
    }

    public static void Store(HashSet<T> set)
    {
      set.Clear();
      _pool.Push(set);
    }

    public static LeaseScope<HashSet<T>> Lease()
    {
      return new LeaseScope<HashSet<T>>(Get(), Store, true);
    }

  }
}