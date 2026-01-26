using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace WrathTools
{
  public static partial class Creators
  {

    /// <remarks> Do not use this name for Named Creators or custom <see cref="ICreator"/>s, only for equality checking and debugging. </remarks>
    public const string DefaultCreatorName = "default";
    /// <remarks> Do not use this name for Named Creators or custom <see cref="ICreator"/>s, only for equality checking and debugging. </remarks>
    public const string ConstructorName = "ctor";

    //TODO: Thread safety
    private static bool _initialized;
    private readonly static object _initializeLock = new();
    private readonly static HashSet<Type> _discoveredConstructors = new();
    private readonly static Dictionary<Type, CreatorCollectionBase> _collections = new();
    private static Dictionary<Type, CreatorCollectionBase> Collections
    {
      get
      {
        Initialize();
        return _collections;
      }
    }

    public static bool HasCreator(this Type type, string name, bool exactArgLength, bool exactArgTypes, bool discoverConstructors, params Type[] argTypes)
      => TryGetCreator(type, out _, name, exactArgLength, exactArgTypes, discoverConstructors, argTypes);

    public static bool HasCreator(this Type type, string name, bool exactArgLength, bool exactArgTypes, params Type[] argTypes)
      => TryGetCreator(type, out _, name, exactArgLength, exactArgTypes, true, argTypes);

    public static bool HasCreator(this Type type, string name, bool discoverConstructors, params Type[] argTypes)
      => TryGetCreator(type, out _, name, false, false, discoverConstructors, argTypes);

    public static bool HasCreator(this Type type, string name, params Type[] argTypes)
      => TryGetCreator(type, out _, name, false, false, true, argTypes);

    public static bool HasCreator(this Type type, bool exactArgLength, bool exactArgTypes, bool discoverConstructors, params Type[] argTypes)
      => TryGetCreator(type, out _, null, exactArgLength, exactArgTypes, discoverConstructors, argTypes);

    public static bool HasCreator(this Type type, bool exactArgLength, bool exactArgTypes, params Type[] argTypes)
      => TryGetCreator(type, out _, null, exactArgLength, exactArgTypes, true, argTypes);

    public static bool HasCreator(this Type type, bool discoverConstructors, params Type[] argTypes)
      => TryGetCreator(type, out _, null, false, false, discoverConstructors, argTypes);

    public static bool HasCreator(this Type type, params Type[] argTypes)
      => TryGetCreator(type, out _, null, false, false, true, argTypes);

    public static bool TryGetCollection(Type type, out ICreatorCollection collection, bool discoverConstructors)
    {
      bool resl = discoverConstructors
        ? TryDiscoverConstructors(type, out CreatorCollectionBase coll)
        : Collections.TryGetValue(type, out coll);
      collection = coll;
      return resl;
    }

    public static bool TryGetCollection(Type type, out ICreatorCollection collection) => TryGetCollection(type, out collection, true);

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
      params Type[] argTypes) 
      => TryGetCreator(type, out creator, name, exactArgLength, exactArgTypes, true, argTypes);

    public static bool TryGetCreator(this Type type, out ICreator creator, string name, bool discoverConstructors, params Type[] argTypes)
      => TryGetCreator(type, out creator, name, false, false, discoverConstructors, argTypes);

    public static bool TryGetCreator(this Type type, out ICreator creator, string name, params Type[] argTypes)
      => TryGetCreator(type, out creator, name, false, false, true, argTypes);

    public static bool TryGetCreator(this Type type, out ICreator creator, bool exactArgLength, bool exactArgTypes, bool discoverConstructors, params Type[] argTypes) 
      => TryGetCreator(type, out creator, null, exactArgLength, exactArgTypes, discoverConstructors, argTypes);

    public static bool TryGetCreator(this Type type, out ICreator creator, bool exactArgLength, bool exactArgTypes, params Type[] argTypes)
      => TryGetCreator(type, out creator, null, exactArgLength, exactArgTypes, true, argTypes);

    public static bool TryGetCreator(this Type type, out ICreator creator, bool discoverConstructors, params Type[] argTypes)
      => TryGetCreator(type, out creator, null, false, false, discoverConstructors, argTypes);

    public static bool TryGetCreator(this Type type, out ICreator creator, params Type[] argTypes)
      => TryGetCreator(type, out creator, false, false, true, argTypes);

    public static ICreatorCollection GetCollection(Type type, bool discoverConstructors)
    {
      if(!TryGetCollection(type, out ICreatorCollection resl, discoverConstructors))
      {
        Diagnostics.LogError(
          new Exception($"Failed to find a Creator Collection for the Type '{type.Name}'"),
          stackTrace: new(true)
        );
      }
      return resl;
    }

    public static ICreatorCollection GetCollection(Type type) => GetCollection(type, true);

    public static ICreator GetCreator(this Type type, string name, bool exactArgLength, bool exactArgTypes, bool discoverConstructors, params Type[] argTypes)
      => GetCollection(type, discoverConstructors).GetCreator(name, exactArgLength, exactArgTypes, argTypes);

    public static ICreator GetCreator(this Type type, string name, bool exactArgLength, bool exactArgTypes, params Type[] argTypes)
      => GetCreator(type, name, exactArgLength, exactArgTypes, true, argTypes);

    public static ICreator GetCreator(this Type type, string name, bool discoverConstructors, params Type[] argTypes)
      => GetCreator(type, name, false, false, discoverConstructors, argTypes);

    public static ICreator GetCreator(this Type type, string name, params Type[] argTypes)
      => GetCreator(type, name, false, false, true, argTypes);

    public static ICreator GetCreator(this Type type, bool exactArgLength, bool exactArgTypes, bool discoverConstructors, params Type[] argTypes)
      => GetCreator(type, null, exactArgLength, exactArgTypes, discoverConstructors, argTypes);

    public static ICreator GetCreator(this Type type, bool exactArgLength, bool exactArgTypes, params Type[] argTypes)
      => GetCreator(type, null, exactArgLength, exactArgTypes, true, argTypes);

    public static ICreator GetCreator(this Type type, bool discoverConstructors, params Type[] argTypes)
      => GetCreator(type, null, false, false, discoverConstructors, argTypes);

    public static ICreator GetCreator(this Type type, params Type[] argTypes)
      => GetCreator(type, null, false, false, true, argTypes);


    private static void Initialize()
    {
      if(_initialized) { return; }
      lock(_initializeLock)
      {
        if(_initialized) { return; }

        bool AttributeCheck(MethodInfo m)
        {
          return m.CustomAttributes.Any(a =>
            (a.AttributeType == typeof(CreatorAttribute) && m.ReturnType == m.DeclaringType)
            || (a.AttributeType == typeof(NamedCreatorAttribute) && m.ReturnType != m.DeclaringType)
          );
        }

        (MethodInfo, string) SelectName(MethodInfo m)
        {
          if(m.CustomAttributes.Any(a => a.AttributeType == typeof(CreatorAttribute) && m.ReturnType == m.DeclaringType))
          {
            return (m, DefaultCreatorName);
          }
          NamedCreatorAttribute attr = m.GetCustomAttribute<NamedCreatorAttribute>();
          if(attr.Name != DefaultCreatorName && attr.Name != ConstructorName)
          {
            return (m, attr.Name);
          }
          return (m, $"invalidCreatorName_{m.DeclaringType.Namespace}.{m.DeclaringType}.{m.Name}");
        }

        Assembly assembly = typeof(Creators).Assembly;
        AssemblyName assemblyName = assembly.GetName();
        IEnumerable<Assembly> relevantAssemblies = AppDomain.CurrentDomain.GetAssemblies()
          .Where(a => a == assembly
            || a.GetReferencedAssemblies().Any(r => AssemblyName.ReferenceMatchesDefinition(r, assemblyName))
          );

        IEnumerable<Type> constructorTypes = relevantAssemblies.SelectMany(a => a.GetTypes())
          .Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(ConstructorsAsCreatorsAttribute)));
        foreach(Type type in constructorTypes)
        {
          //TODO: Open Generic Constructor Factories 
          if(type.IsGenericTypeDefinition) { continue; }
          TryDiscoverConstructors(type, out _);
        }

        //TODO: Open Generic Creator factories
        IEnumerable<(MethodInfo, string)> creatorMethods = relevantAssemblies.SelectMany(a => a.GetTypes())
          .SelectMany(t => t.GetMethods())
          .Where(m => m.IsStatic && !m.IsGenericMethod && m.IsPublic && AttributeCheck(m))
          .Select(m => SelectName(m));
        foreach((MethodInfo method, string name) in creatorMethods)
        {
          BuildCreators(method.ReturnType, method.GetParameters(), (ex) => Expression.Call(method, ex), name);
        }

        _initialized = true;
      }
    }

    private static bool TryDiscoverConstructors(Type type, out CreatorCollectionBase collection)
    {
      if(_discoveredConstructors.Contains(type))
      {
        return Collections.TryGetValue(type, out collection);
      }
      _discoveredConstructors.Add(type);
      ConstructorInfo[] constructors = type.GetConstructors();
      if(constructors.Length == 0)
      {
        return Collections.TryGetValue(type, out collection);
      }
      collection = GetOrCreateCollection(type);
      foreach(ConstructorInfo constructor in constructors)
      {
        BuildCreators(type, constructor.GetParameters(), (ex) => Expression.New(constructor, ex), ConstructorName);
      }
      return true;
    }

    private static CreatorCollectionBase GetOrCreateCollection(Type type)
    {
      if(!_collections.TryGetValue(type, out CreatorCollectionBase collection))
      {
        collection = (CreatorCollectionBase)_collectionCreator.MakeGenericMethod(type).Invoke(null, null);
        _collections[type] = collection;
      }
      return collection;
    }

    private static void BuildCreators(Type type, ParameterInfo[] parameters, Func<Expression[], Expression> call, string name)
    {
      CreatorCollectionBase collection = GetOrCreateCollection(type);
      ParameterExpression[] allParameters = new ParameterExpression[parameters.Length];
      for(int i = 0; i < parameters.Length; i++)
      {
        allParameters[i] = Expression.Parameter(parameters[i].ParameterType, parameters[i].Name);
      }
      Expression[] callExpressions = new Expression[parameters.Length];
      for(int d = 0; d <= parameters.Length; d++)
      {
        if(d < parameters.Length && !parameters[d].HasDefaultValue) { continue; }
        Type[] invokeTypes = new Type[d + 1];
        ParameterExpression[] lambdaArgs = new ParameterExpression[parameters.Length - d];
        invokeTypes[^1] = type;
        for(int i = 0; i < parameters.Length; i++)
        {
          if(i < d)
          {
            callExpressions[i] = allParameters[i];
            invokeTypes[i] = parameters[i].ParameterType;
          }
          else
          {
            callExpressions[i] = Expression.Constant(parameters[i].DefaultValue, parameters[i].ParameterType);
            lambdaArgs[i - d] = allParameters[i];
          }
        }
        ICreator creator = (ICreator)_creatorsByArity[d]
          .MakeGenericMethod(invokeTypes)
          .Invoke(null, new object[] { call.Invoke(callExpressions), lambdaArgs, name });
        collection.AddCreatorInternal(creator);
      }
    }

  }
}