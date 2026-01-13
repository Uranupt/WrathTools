using System;
using System.Collections.Generic;


namespace WrathTools
{
  public static class ListTools
  {

    public static List<T> NewWhere<T>(this List<T> source, Func<T, bool> predicate)
    {
      List<T>  resl= new();
      resl.AddWhere(source, predicate);
      return resl;
    }

    public static void AddWhere<T>(this List<T> destination, List<T> source, Func<T, bool> predicate)
    {
      foreach(T item in source)
      {
        if(predicate(item))
        {
          destination.Add(item);
        }
      }
    }

    public static void ReplaceWhere<T>(this List<T> destination, List<T> source, Func<T, bool> predicate)
    {
      destination.Clear();
      destination.AddWhere(source, predicate);
    }

    public static void IndexWise<T>(this IList<T> source, Action<T, int> operation)
    {
      for(int i = 0; i < source.Count; i++)
      {
        operation.Invoke(source[i], i);
      }
    }

    public static bool IsIdenticalTo<T>(this IList<T> source, IList<T> other)
    {
      if(source.Count != other.Count) 
      { 
        return false; 
      }
      for(int i = 0; i < source.Count; i++)
      {
        if(!source[i].Equals(other[i]))
        {
          return false;
        }
      }
      return true;
    }

    public static bool IsTrueForAny<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
      foreach(T item in source)
      {
        if(predicate(item))
        {
          return true;
        }
      }
      return false;
    }

    public static bool IsTrueForAll<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
      foreach(T item in source)
      {
        if(!predicate(item))
        {
          return false;
        }
      }
      return true;
    }

  }
}