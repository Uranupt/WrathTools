using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;


namespace WrathTools
{
  public static partial class Creators
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

    public static bool TryGetCollection(Type type, out ICreatorCollection collection, bool discoverConstructors)
    {
      bool resl = discoverConstructors
        ? TryDiscoverConstructors(type, out CreatorCollectionBase coll)
        : Collections.TryGetValue(type, out coll);
      collection = coll;
      return resl;
    }

    public static bool TryGetCollection(Type type, out ICreatorCollection collection) => TryGetCollection(type, out collection, false);

    public static bool TryGetCreator(this Type type, out ICreator creator, string name,
      bool exactArgLength, bool exactArgTypes, bool discoverConstructors, params Type[] argTypes)
    {
      if(TryGetCollection(type, out ICreatorCollection collection, discoverConstructors))
      {
        return collection.TryGetCreator(out creator, name, exactArgLength, exactArgTypes, argTypes);
      }
      creator = null;
      return false;
    }

    public static bool TryGetCreator(this Type type, out ICreator creator, string name, bool exactArgLength, bool exactArgTypes,
      params Type[] argTypes) => TryGetCreator(type, out creator, name, exactArgLength, exactArgTypes, false, argTypes);

    public static bool TryGetCreator(this Type type, out ICreator creator, string name, bool discoverConstructors, params Type[] argTypes)
      => TryGetCreator(type, out creator, name, false, false, discoverConstructors, argTypes);

    public static bool TryGetCreator(this Type type, out ICreator creator, string name, params Type[] argTypes)
      => TryGetCreator(type, out creator, name, false, false, false, argTypes);

    public static bool TryGetCreator(this Type type, out ICreator creator, bool exactArgLength, bool exactArgTypes,
      bool discoverConstructors, params Type[] argTypes) => TryGetCreator(type, out creator, exactArgLength, exactArgTypes, discoverConstructors, argTypes);

    public static bool TryGetCreator(this Type type, out ICreator creator, bool exactArgLength, bool exactArgTypes, params Type[] argTypes)
      => TryGetCreator(type, out creator, exactArgLength, exactArgTypes, argTypes);

    public static bool TryGetCreator(this Type type, out ICreator creator, bool discoverConstructors, params Type[] argTypes)
      => TryGetCreator(type, out creator, discoverConstructors, argTypes);

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

    private static bool TryDiscoverConstructors(Type type, out CreatorCollectionBase collection)
    {
      if(_discoveredConstructors.Contains(type))
      {
        return Collections.TryGetValue(type, out collection);
      }
      _discoveredConstructors.Add(type);

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