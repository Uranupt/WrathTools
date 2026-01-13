using System.Collections;
using System.Collections.Generic;
using System;


namespace WrathTools
{ 
  public static class Enumeration
  {

    public static IEnumerable<T> NonNullEnumerable<T>(ICollection<T> collection) => PredicateEnumerable(collection, i => i != null);

    public static IEnumerable<T> PredicateEnumerable<T>(ICollection<T> collection, Func<T, bool> predicate)
    {
      foreach(T item in collection)
      {
        if(predicate.Invoke(item))
        {
          yield return item;
        }
      }
    }

    public static IEnumerable<T2> SelectionEnumerable<T1, T2>(ICollection<T1> collection, Func<T1, T2> selection)
    {
      foreach(T1 item in NonNullEnumerable(collection))
      {
        yield return selection.Invoke(item);
      }
    }

    public static IEnumerable<T2> SelectionEnumerable<T1, T2>(ICollection<T1> collection, Func<T1, T2> selection, Func<T1, bool> predicate)
    {
      foreach(T1 item in PredicateEnumerable(collection, predicate))
      {
        yield return selection.Invoke(item);
      }
    }


    public static IEnumerator<T> NonNullEnumerator<T>(ICollection<T> collection) => NonNullEnumerable(collection).GetEnumerator();
    public static IEnumerator<T> PredicateEnumerator<T>(ICollection<T> collection, Func<T, bool> predicate)
      => PredicateEnumerable(collection, predicate).GetEnumerator();
    public static IEnumerator<T2> SelectionEnumerator<T1, T2>(ICollection<T1> collection, Func<T1, T2> selection)
      => SelectionEnumerable(collection, selection).GetEnumerator();
    public static IEnumerator<T2> SelectionEnumerator<T1, T2>(ICollection<T1> collection, Func<T1, T2> selection, Func<T1, bool> predicate)
      => SelectionEnumerable(collection, selection, predicate).GetEnumerator();

  }
}
