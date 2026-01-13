using System.Collections.Generic;


namespace WrathTools
{
  public readonly struct CollectionDelta<T>
  {

    public readonly IReadOnlyCollection<T> Added;
    public readonly IReadOnlyCollection<T> Removed;

    public CollectionDelta(IEnumerable<T> current, IEnumerable<T> old)
    {
      HashSet<T> added = new(current);
      HashSet<T> removed = new(old);
      added.ExceptWith(old);
      removed.ExceptWith(current);
      Added = added;
      Removed = removed;
    }

    public CollectionDelta(IEnumerable<T> items, bool added)
    {
      if(added)
      {
        Added = new HashSet<T>(items);
        Removed = new HashSet<T>();
      }
      else
      {
        Added = new HashSet<T>();
        Removed = new HashSet<T>(items);
      }
    }

    public CollectionDelta(T item, bool added)
    {
      if(added)
      {
        Added = new HashSet<T>() { item };
        Removed = new HashSet<T>();
      }
      else
      {
        Added = new HashSet<T>();
        Removed = new HashSet<T>() { item };
      }
    }

  }
}