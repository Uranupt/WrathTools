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
    public const string ConstructorName = "ctor";

    private static bool _initialized;
    private static Dictionary<Type, CreatorCollectionBase> _collections = new();
    private static Dictionary<Type, CreatorCollectionBase> Collections
    {
      get
      {
        Initialize();
        return _collections;
      }
    }

    private static MethodInfo _newCreatorInfo = typeof(Creators).GetMethod("NewCreator");
    private static MethodInfo _newConstructorInfo = typeof(Creators).GetMethod("GenericConstructorCreator", BindingFlags.Static);

    public static bool TryGetCollection(this Type type, out ICreator creator, bool create = false)
    {

    }

    public static bool TryGetCreator(this Type type, out ICreator creator, string name,
      bool exactArgLength, bool exactArgTypes, bool discoverConstructors, params Type[] argTypes)
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