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

    private static bool _initialized;
    private readonly static object _initializeLock = new();
    private readonly static HashSet<Type> _discoveredConstructors = new();
    private readonly static Dictionary<Type, CreatorCollectionBase> _collections = new();
    private readonly static Dictionary<Type, CreatorFactory> _factories = new();

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
      if(!TryGetCollection(type, out ICreatorCollection collection, discoverConstructors)
        || !collection.TryGetCreator(out creator, name, exactArgLength, exactArgTypes, argTypes))
      {
        if(type.IsGenericType && _factories.TryGetValue(type.GetGenericTypeDefinition(), out CreatorFactory factory))
        {
          factory.Build(type);
          if(TryGetCollection(type, out collection, discoverConstructors)
            && collection.TryGetCreator(out creator, name, exactArgLength, exactArgTypes, argTypes))
          {
            return true;
          }
        }
        creator = null;
        return false;
      }
      return true;
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


    public static ICreator GetCreator(this Type type, string name, bool exactArgLength, bool exactArgTypes, bool discoverConstructors, params Type[] argTypes)
    {
      if(!TryGetCreator(type, out ICreator creator, name, exactArgLength, exactArgTypes, discoverConstructors, argTypes))
      {
        Diagnostics.LogError(
          new Exception($"Failed to find a Creator for Type: '{type.Name}' with Argument Types: {ArgsSignature.GetTypesString(argTypes)}. " +
          $"Fetch settings: [name = {name ?? "(none)"}], [exactArgLength = {exactArgLength}], [exactArgTypes = {exactArgTypes}], [discoverConstructors = {discoverConstructors}] "),
          stackTrace: new(true)
        );
      }
      return creator;
    }

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
      Diagnostics.LogMessage("Attempting to Initialize");
      if(_initialized) { return; }
      lock(_initializeLock)
      {
        if(_initialized) { return; }
        Diagnostics.LogMessage("Running initialize");
        bool AttributeCheck(MethodInfo m)
        {
          if(m.ReturnType.GetGenericArguments().Length > 0
            && m.ReturnType.ContainsGenericParameters
            && m.ReturnType.GetGenericArguments().Length != m.DeclaringType.GetGenericArguments().Length)
          {
            return false;
          }
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
        HashSet<Type> factoryTypes = new();
        foreach(Type type in constructorTypes)
        {
          if(type.IsGenericType)
          {
            if(type.IsGenericTypeDefinition)
            {
              _factories[type] = new CreatorFactory(type);
            }
            else if(!type.ContainsGenericParameters)
            {
              factoryTypes.Add(type);
            }
            continue;
          }
          TryDiscoverConstructors(type, out _);
        }

        IEnumerable<(MethodInfo, string)> creatorMethods = relevantAssemblies.SelectMany(a => a.GetTypes())
          .SelectMany(t => t.GetMethods())
          .Where(m => m.IsStatic && !m.IsGenericMethod && m.IsPublic && AttributeCheck(m))
          .Select(m => SelectName(m));
        foreach((MethodInfo method, string name) in creatorMethods)
        {
          if(method.DeclaringType.IsGenericType)
          {
            if(method.DeclaringType.IsGenericTypeDefinition)
            {
              if(!method.ReturnType.IsGenericType || !method.ReturnType.ContainsGenericParameters
                && !method.GetParameters().Any(p => p.ParameterType.IsGenericTypeParameter))
              {
                BuildCreators(method.ReturnType, method.GetParameters(), (ex) => Expression.Call(method, ex), name);
              }
              else
              {
                Type genType = method.ReturnType.GetGenericTypeDefinition();
                if(!_factories.TryGetValue(genType, out CreatorFactory factory))
                {
                  factory = new CreatorFactory(genType);
                  _factories[genType] = factory;
                }
                factory.AddTemplate(name, method.DeclaringType, method);
              }
            }
            else if(!method.DeclaringType.ContainsGenericParameters && method.ReturnType.IsGenericType)
            {
              factoryTypes.Add(method.ReturnType);
            }
            continue;
          }
          BuildCreators(method.ReturnType, method.GetParameters(), (ex) => Expression.Call(method, ex), name);
        }

        foreach(Type type in factoryTypes)
        {
          if(_factories.TryGetValue(type.GetGenericTypeDefinition(), out CreatorFactory factory))
          {
            factory.Build(type);
          }
        }

        Diagnostics.LogMessage("Finished initialize");
        _initialized = true;
      }
    }

    internal static bool TryDiscoverConstructors(Type type, out CreatorCollectionBase collection)
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

    internal static CreatorCollectionBase GetOrCreateCollection(Type type)
    {
      if(!_collections.TryGetValue(type, out CreatorCollectionBase collection))
      {
        collection = (CreatorCollectionBase)_collectionCreator.MakeGenericMethod(type).Invoke(null, null);
        _collections[type] = collection;
      }
      return collection;
    }

    internal static void BuildCreators(Type type, ParameterInfo[] parameters, Func<Expression[], Expression> call, string name)
    {
      //This exists as an artifact of the commented out default value overloading. Leaving it in for if I ever find a deterministic
      //way to prevent overload collision
      CreatorCollectionBase collection = GetOrCreateCollection(type);
      ParameterExpression[] allParameters = new ParameterExpression[parameters.Length];
      Type[] invokeTypes = new Type[allParameters.Length + 1];
      for(int i = 0; i < parameters.Length; i++)
      {
        allParameters[i] = Expression.Parameter(parameters[i].ParameterType, parameters[i].Name);
        invokeTypes[i] = parameters[i].ParameterType;
      }
      invokeTypes[^1] = type;
      ICreator creator = (ICreator)_creatorsByArity[parameters.Length]
        .MakeGenericMethod(invokeTypes)
        .Invoke(null, new object[] { call.Invoke(allParameters), allParameters, name });
      collection.AddCreatorInternal(creator);
      //Expression[] callExpressions = new Expression[parameters.Length];
      //for(int d = 0; d <= parameters.Length; d++)
      //{
      //  if(d < parameters.Length && !parameters[d].HasDefaultValue) { continue; }
      //  Type[] invokeTypes = new Type[d + 1];
      //  ParameterExpression[] lambdaArgs = new ParameterExpression[d];
      //  invokeTypes[^1] = type;
      //  for(int i = 0; i < parameters.Length; i++)
      //  {
      //    if(i < d)
      //    {
      //      callExpressions[i] = allParameters[i];
      //      invokeTypes[i] = parameters[i].ParameterType;
      //      lambdaArgs[i] = allParameters[i];
      //    }
      //    else
      //    {
      //      callExpressions[i] = Expression.Constant(parameters[i].DefaultValue, parameters[i].ParameterType);
      //    }
      //  }
      //  ICreator creator = (ICreator)_creatorsByArity[d]
      //    .MakeGenericMethod(invokeTypes)
      //    .Invoke(null, new object[] { call.Invoke(callExpressions), lambdaArgs, name });
      //  collection.AddCreatorInternal(creator);
      //}
    }

  }
}