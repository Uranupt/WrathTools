using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;


namespace WrathTools
{
  public static class Creators
  {

    private static bool _initialized;
    private static Dictionary<Type, ICreator> _selfCreators = new();
    private static Dictionary<Type, ICreator> SelfCreators
    {
      get
      {
        Initialize();
        return _selfCreators;
      }
    }
    private static MethodInfo _newCreatorInfo = typeof(Creators).GetMethod("NewCreator");

    public static bool HasCreator(this Type type) => SelfCreators.ContainsKey(type);
    public static bool HasCreator<T>() => HasCreator(typeof(T));

    public static bool TryCreateEnumerable<T, TItem>(IEnumerable<TItem> values, out T enumerable) where T : IEnumerable
    {

    }

    public static T[] CreateArray<T>(IEnumerable<T> values)
    {
      T[] resl = new T[](values);
    }

    public static bool TryGetCreator(this Type type, out ICreator creator)
    {
      return SelfCreators.TryGetValue(type, out creator);
    }

    public static bool TryGetCreator<T>(out ICreator<T> creator)
    {
      if(TryGetCreator(typeof(T), out ICreator ctr))
      {
        creator = (ICreator<T>)ctr;
        return true;
      }
      creator = null;
      return false;
    }

    public static bool TryCreate(this Type type, out object value)
    {
      if(TryGetCreator(type, out ICreator creator))
      {
        value = creator.Create();
        return true;
      }
      value = default;
      return false;
    }

    public static bool TryCreate<T>(out T value)
    {
      if(TryGetCreator<T>(out ICreator<T> creator))
      {
        value = creator.Create();
        return true;
      }
      value = default;
      return false;
    }

    public static object Create(this Type type)
    {
      if(!TryCreate(type, out object value))
      {
        Diagnostics.LogError(
          new InvalidOperationException(InvalidCreateType(type.Name)),
          stackTrace: new(true)
        );
      }
      return value;
    }

    public static T Create<T>()
    {
      if(!TryCreate(out T value))
      {
        Diagnostics.LogError(
          new InvalidOperationException(InvalidCreateType(typeof(T).Name)),
          stackTrace: new(true)
        );
      }
      return value;
    }

    private static void Initialize()
    {
      if(_initialized) { return; }
      _initialized = true;
      MethodInfo[] methods = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a => a.GetTypes())
        .SelectMany(t => t.GetMethods())
        .Where(m => m.IsStatic && !m.IsGenericMethod
          && m.GetCustomAttribute<SelfCreatorAttribute>() != null
          && m.GetParameters().Length == 0
          && m.ReturnType == m.DeclaringType)
        .ToArray();
      foreach(MethodInfo method in methods)
      {
        ICreator creator = (ICreator)_newCreatorInfo.MakeGenericMethod(method.DeclaringType).Invoke(null, new object[] { method });
        _selfCreators[method.DeclaringType] = creator; 
      }
    }

    private static ICreator NewCreator<T>(MethodInfo createMethod)
    { 
      Func<T> create = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), createMethod);
      return new Creator<T>(create);
    }

    private static string InvalidCreateType(string typeName) => $"Failed to find an ICreator instance for the  Type '{typeName}'";

  }
}