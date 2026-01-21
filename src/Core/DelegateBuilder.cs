using System;
using System.Reflection;


namespace WrathTools
{
  public static class DelegateBuilder
  {

    public static Func<T> Func<T>(MethodInfo info)
    {
      return (System.Func<T>)Delegate.CreateDelegate(typeof(System.Func<T>), info);
    }

    public static Func<T1, T2> Func<T1, T2>(MethodInfo info)
    {
      return (System.Func<T1, T2>)Delegate.CreateDelegate(typeof(System.Func<T1, T2>), info);
    }

    public static Action Action(MethodInfo info)
    {
      return (System.Action)Delegate.CreateDelegate(typeof(System.Action), info);
    }

    public static Action<T> Action<T>(MethodInfo info)
    {
      return (System.Action<T>)Delegate.CreateDelegate(typeof(System.Action<T>), info);
    }

    public static Action<T1, T2> Action<T1, T2>(MethodInfo info)
    {
      return (System.Action<T1, T2>)Delegate.CreateDelegate(typeof(System.Action<T1, T2>), info);
    }

  }
}