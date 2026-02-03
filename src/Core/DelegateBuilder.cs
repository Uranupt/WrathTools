using System;
using System.Reflection;
using System.Linq.Expressions;


namespace WrathTools
{
  public static class DelegateBuilder
  {

    public static TDelegate CompileLambda<TDelegate>(Expression body, ParameterExpression[] parameters, bool noJIT) where TDelegate : Delegate
    {
      return (TDelegate)Expression.Lambda(body, parameters).Compile(noJIT);
    }

    //1
    public static Func<TResult> 
      Func<TResult>(MethodInfo info)
    {
      return (System.Func<TResult>)Delegate.CreateDelegate(
        typeof(System.Func<TResult>), 
        info
      );
    }

    //2
    public static Func<T, TResult> 
      Func<T, TResult>(MethodInfo info)
    {
      return (System.Func<T, TResult>)Delegate.CreateDelegate(
        typeof(System.Func<T, TResult>), 
        info
      );
    }

    //3
    public static Func<T1, T2, TResult>
      Func<T1, T2, TResult>(MethodInfo info)
    {
      return (System.Func<T1, T2, TResult>)Delegate.CreateDelegate(
        typeof(System.Func<T1, T2, TResult>),
        info
      );
    }

    //4
    public static Func<T1, T2, T3, TResult>
      Func<T1, T2, T3, TResult>(MethodInfo info)
    {
      return (System.Func<T1, T2, T3, TResult>)Delegate.CreateDelegate(
        typeof(System.Func<T1, T2, T3, TResult>),
        info
      );
    }

    //5
    public static Func<T1, T2, T3, T4, TResult>
      Func<T1, T2, T3, T4, TResult>(MethodInfo info)
    {
      return (System.Func<T1, T2, T3, T4, TResult>)Delegate.CreateDelegate(
        typeof(System.Func<T1, T2, T3, T4, TResult>),
        info
      );
    }

    //6
    public static Func<T1, T2, T3, T4, T5, TResult>
      Func<T1, T2, T3, T4, T5, TResult>(MethodInfo info)
    {
      return (System.Func<T1, T2, T3, T4, T5, TResult>)Delegate.CreateDelegate(
        typeof(System.Func<T1, T2, T3, T4, T5, TResult>),
        info
      );
    }

    //7
    public static Func<T1, T2, T3, T4, T5, T6, TResult>
      Func<T1, T2, T3, T4, T5, T6, TResult>(MethodInfo info)
    {
      return (System.Func<T1, T2, T3, T4, T5, T6, TResult>)Delegate.CreateDelegate(
        typeof(System.Func<T1, T2, T3, T4, T5, T6, TResult>),
        info
      );
    }

    //8
    public static Func<T1, T2, T3, T4, T5, T6, T7, TResult>
      Func<T1, T2, T3, T4, T5, T6, T7, TResult>(MethodInfo info)
    {
      return (System.Func<T1, T2, T3, T4, T5, T6, T7, TResult>)Delegate.CreateDelegate(
        typeof(System.Func<T1, T2, T3, T4, T5, T6, T7, TResult>),
        info
      );
    }

    //9
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
      Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(MethodInfo info)
    {
      return (System.Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)Delegate.CreateDelegate(
        typeof(System.Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>),
        info
      );
    }

    //10
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(MethodInfo info)
    {
      return (System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)Delegate.CreateDelegate(
        typeof(System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>),
        info
      );
    }

    //11
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(MethodInfo info)
    {
      return (System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)Delegate.CreateDelegate(
        typeof(System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>),
        info
      );
    }

    //12
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(MethodInfo info)
    {
      return (System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>)Delegate.CreateDelegate(
        typeof(System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>),
        info
      );
    }

    //13
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(MethodInfo info)
    {
      return (System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>)Delegate.CreateDelegate(
        typeof(System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>),
        info
      );
    }

    //14
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(MethodInfo info)
    {
      return (System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>)Delegate.CreateDelegate(
        typeof(System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>),
        info
      );
    }

    //15
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(MethodInfo info)
    {
      return (System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>)Delegate.CreateDelegate(
        typeof(System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>),
        info
      );
    }

    //16
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
      Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(MethodInfo info)
    {
      return (System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>)Delegate.CreateDelegate(
        typeof(System.Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>),
        info
      );
    }


    //0
    public static Action 
      Action(MethodInfo info)
    {
      return (System.Action)Delegate.CreateDelegate(
        typeof(System.Action), 
        info
      );
    }

    //1
    public static Action<T>
      Action<T>(MethodInfo info)
    {
      return (System.Action<T>)Delegate.CreateDelegate(
        typeof(System.Action<T>),
        info
      );
    }

    //2
    public static Action<T1, T2>
      Action<T1, T2>(MethodInfo info)
    {
      return (System.Action<T1, T2>)Delegate.CreateDelegate(
        typeof(System.Action<T1, T2>),
        info
      );
    }

    //3
    public static Action<T1, T2, T3>
      Action<T1, T2, T3>(MethodInfo info)
    {
      return (System.Action<T1, T2, T3>)Delegate.CreateDelegate(
        typeof(System.Action<T1, T2, T3>),
        info
      );
    }

    //4
    public static Action<T1, T2, T3, T4>
      Action<T1, T2, T3, T4>(MethodInfo info)
    {
      return (System.Action<T1, T2, T3, T4>)Delegate.CreateDelegate(
        typeof(System.Action<T1, T2, T3, T4>),
        info
      );
    }

    //5
    public static Action<T1, T2, T3, T4, T5>
      Action<T1, T2, T3, T4, T5>(MethodInfo info)
    {
      return (System.Action<T1, T2, T3, T4, T5>)Delegate.CreateDelegate(
        typeof(System.Action<T1, T2, T3, T4, T5>),
        info
      );
    }

    //6
    public static Action<T1, T2, T3, T4, T5, T6>
      Action<T1, T2, T3, T4, T5, T6>(MethodInfo info)
    {
      return (System.Action<T1, T2, T3, T4, T5, T6>)Delegate.CreateDelegate(
        typeof(System.Action<T1, T2, T3, T4, T5, T6>),
        info
      );
    }

    //7
    public static Action<T1, T2, T3, T4, T5, T6, T7>
      Action<T1, T2, T3, T4, T5, T6, T7>(MethodInfo info)
    {
      return (System.Action<T1, T2, T3, T4, T5, T6, T7>)Delegate.CreateDelegate(
        typeof(System.Action<T1, T2, T3, T4, T5, T6, T7>),
        info
      );
    }

    //8
    public static Action<T1, T2, T3, T4, T5, T6, T7, T8>
      Action<T1, T2, T3, T4, T5, T6, T7, T8>(MethodInfo info)
    {
      return (System.Action<T1, T2, T3, T4, T5, T6, T7, T8>)Delegate.CreateDelegate(
        typeof(System.Action<T1, T2, T3, T4, T5, T6, T7, T8>),
        info
      );
    }

    //9
    public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>
      Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>(MethodInfo info)
    {
      return (System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>)Delegate.CreateDelegate(
        typeof(System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>),
        info
      );
    }

    //10
    public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
      Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(MethodInfo info)
    {
      return (System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>)Delegate.CreateDelegate(
        typeof(System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>),
        info
      );
    }

    //11
    public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
      Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(MethodInfo info)
    {
      return (System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>)Delegate.CreateDelegate(
        typeof(System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>),
        info
      );
    }

    //12
    public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
      Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(MethodInfo info)
    {
      return (System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>)Delegate.CreateDelegate(
        typeof(System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>),
        info
      );
    }

    //13
    public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
      Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(MethodInfo info)
    {
      return (System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>)Delegate.CreateDelegate(
        typeof(System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>),
        info
      );
    }

    //14
    public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
      Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(MethodInfo info)
    {
      return (System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>)Delegate.CreateDelegate(
        typeof(System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>),
        info
      );
    }

    //15
    public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
      Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(MethodInfo info)
    {
      return (System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>)Delegate.CreateDelegate(
        typeof(System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>),
        info
      );
    }

    //16
    public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
      Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(MethodInfo info)
    {
      return (System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>)Delegate.CreateDelegate(
        typeof(System.Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>),
        info
      );
    }

  }
}