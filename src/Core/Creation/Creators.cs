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

    /// <remarks> Do not use this name for custom <see cref="ICreator"/>s, only for equality checking and debugging. </remarks>
    public const string DefaultCreatorName = "default";
    /// <remarks> Do not use this name for custom <see cref="ICreator"/>s, only for equality checking and debugging. </remarks>
    public const string ConstructorName = "ctr";

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
    private static MethodInfo _newConstructorInfo = typeof(Creators).GetMethod("GenericConstructorCreator", BindingFlags.Static);

    public static bool HasCreator(this Type type, bool includeNew = false)
    {
      if(SelfCreators.ContainsKey(type)) { return true; }
      return includeNew && type.GetConstructor(Type.EmptyTypes) != null;
    }
    public static bool HasCreator<T>() => HasCreator(typeof(T));

    public static bool HasCreatorWithParams(this Type type, params Type[] paramTypes) => HasCreatorWithParams(type, false, paramTypes);

    public static bool HasCreatorWithParams(this Type type, bool includeNew, params Type[] paramTypes)
    {
      if(paramTypes == null || paramTypes.Length == 0) { return HasCreator(type, includeNew); }
    }


    public static bool TryGetCreator(this Type type, out ICreator creator, bool includeNew = false)
    {
      if(!SelfCreators.TryGetValue(type, out creator))
      {
        if(includeNew)
        {
          ConstructorInfo newInfo = type.GetConstructor(Type.EmptyTypes);
          if(newInfo != null)
          {
            creator = NewConstructorCreator(type, newInfo);
            _selfCreators[type] = creator;
          }
        }
      }
      return creator != null;
    }

    public static bool TryGetCreator<T>(out ICreator<T> creator, bool includeNew = false)
    {
      if(TryGetCreator(typeof(T), out ICreator ctr, includeNew))
      {
        creator = (ICreator<T>)ctr;
        return true;
      }
      creator = null;
      return false;
    }

    public static ICreator GetCreator(this Type type)
    {
      if(!TryGetCreator(type, out ICreator ctr, true))
      {
        Diagnostics.LogError(
          new InvalidOperationException(InvalidCreateType(type.Name)),
          stackTrace: new(true)
        );
      }
      return ctr;
    }

    public static ICreator<T> GetCreator<T>()
    {
      if(!TryGetCreator(out ICreator<T> ctr, true))
      {
        Diagnostics.LogError(
          new InvalidOperationException(InvalidCreateType(typeof(T).Name)),
          stackTrace: new(true)
        );
      }
      return ctr;
    }

    public static bool TryCreate(this Type type, out object value, bool includeNew = false)
    {
      if(TryGetCreator(type, out ICreator creator, includeNew))
      {
        value = creator.Create();
        return true;
      }
      value = default;
      return false;
    }

    public static bool TryCreate<T>(out T value, bool includeNew = false)
    {
      if(TryGetCreator<T>(out ICreator<T> creator, includeNew))
      {
        value = creator.Create();
        return true;
      }
      value = default;
      return false;
    }

    public static object Create(this Type type)
    {
      if(!TryCreate(type, out object value, true))
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
      if(!TryCreate(out T value, true))
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
      //TODO: Clean Up Reflection, pare to only marked relevant assemblies.
      if(_initialized) { return; }
      _initialized = true;
      MethodInfo[] methods = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a => a.GetTypes())
        .SelectMany(t => t.GetMethods())
        .Where(m => m.IsStatic && !m.IsGenericMethod
          && m.GetCustomAttribute<CreatorAttribute>() != null
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

    private static ICreator NewConstructorCreator(Type type, ConstructorInfo info)
    {
      return (ICreator)_newConstructorInfo.MakeGenericMethod(type).Invoke(null, new object[] { info });
    }

    private static ICreator GenericConstructorCreator<T>(ConstructorInfo info)
    {
      return new Creator<T>(Expression.Lambda<Func<T>>(Expression.New(info)).Compile());
    }

    private static string InvalidCreateType(string typeName) => $"Failed to find an ICreator instance for the  Type '{typeName}'";

  }
}