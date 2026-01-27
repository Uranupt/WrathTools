using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace WrathTools
{
  public static partial class BinarySerialization
  {

    internal const string DefaultConverterName = "default";

    private static bool _initialized;
    private static MethodInfo _enumerableBuilder = typeof(BinarySerialization).GetMethod("BuildEnumerableConverter", BindingFlags.Static | BindingFlags.NonPublic);
    private static readonly object _initializeLock = new();
    private static readonly object _enumerableLock = new();
    private static readonly object _buildLock = new();

    private static readonly Dictionary<Type, BinaryConverterCollection> _collections = new();
    private static readonly Dictionary<Type, BinaryConverterFactory> _factories = new();

    internal static readonly MethodInfo ManualConverterBuilder = typeof(BinarySerialization).GetMethod("BuildManualConverter", BindingFlags.Static | BindingFlags.NonPublic);
    internal static readonly MethodInfo SchemaConverterBuilder = typeof(BinarySerialization).GetMethod("BuildSchemaConverter", BindingFlags.Static | BindingFlags.NonPublic);

    internal readonly static HashSet<Type> SystemSerialzableTypes = new()
    {
      typeof(bool),
      typeof(byte),
      typeof(sbyte),
      typeof(short),
      typeof(ushort),
      typeof(int),
      typeof(uint),
      typeof(long),
      typeof(ulong),
      typeof(float),
      typeof(double),
      typeof(decimal),
      typeof(char),
      typeof(string)
    };

    private static Dictionary<Type, BinaryConverterCollection> Collections
    {
      get
      {
        Initialize();
        return _collections;
      }
    }

    internal static bool IsBaseTypeSerializable(Type type, IEnumerable<Type> includedTypes = null)
    {
      if(type.IsArray)
      {
        return IsBaseTypeSerializable(type.GetElementType(), includedTypes);
      }
      IEnumerable<Type> iEnumTypes = type.GetEnumerableTypes();
      int count = iEnumTypes.Count();
      if(count > 1) { return false; }
      else if(count == 0)
      {
        return type.IsBinarySerializable() ||
          (includedTypes != null && includedTypes.Contains(type));
      }
      return IsBaseTypeSerializable(iEnumTypes.First().GenericTypeArguments[0], includedTypes);
    }

    internal static bool TrySelectSerializerMethods(Type declaringType, Type targetedType, out MethodInfo read, out MethodInfo write)
    {
      read = null;
      write = null;
      foreach(MethodInfo method in declaringType.GetMethods())
      {
        if(method.Name == "Read")
        {
          ParameterInfo[] parameters = method.GetParameters();
          if(parameters.Length == 1
            && parameters[0].ParameterType == typeof(BinaryReader)
            && method.ReturnType == targetedType)
          {
            read = method;
          }
        }
        else if(method.Name == "Write")
        {
          ParameterInfo[] parameters = method.GetParameters();
          if(parameters.Length == 2
            && parameters[0].ParameterType == typeof(BinaryWriter)
            && parameters[1].ParameterType == targetedType)
          {
            write = method;
          }
        }
      }
      return read != null && write != null;
    }

    private static void Initialize()
    {
      if(_initialized) { return; }
      lock(_initializeLock)
      {
        if(_initialized) { return; }

        static IEnumerable<BinarySerializerBuildInfo> GetBuildInfos(Type t)
        {
          foreach(BinarySerializableAttribute attr in t.GetCustomAttributes<BinarySerializableAttribute>())
          {
            yield return new BinarySerializerBuildInfo(t, attr);
          }
          foreach(BinarySerializerAttribute attr in t.GetCustomAttributes<BinarySerializerAttribute>())
          {
            yield return new BinarySerializerBuildInfo(t, attr);
          }
        }

        InitializeSystemTypes();
        Assembly assembly = typeof(BinarySerialization).Assembly;
        AssemblyName assemblyName = assembly.GetName();
        IEnumerable<Assembly> relevantAssemblies = AppDomain.CurrentDomain.GetAssemblies()
          .Where(a => a == assembly
            || a.GetReferencedAssemblies().Any(r => AssemblyName.ReferenceMatchesDefinition(r, assemblyName))
          );

        BinarySerializerBuildInfo[] buildInfos = relevantAssemblies.SelectMany(a => a.GetTypes())
          .Where(t => t.CustomAttributes.Any(a => a.AttributeType == typeof(BinarySerializableAttribute)
            || a.AttributeType == typeof(BinarySerializerAttribute))
          )
          .SelectMany(t => GetBuildInfos(t))
          .ToArray();

        HashSet<Type> autoTypes = new();
        HashSet<BinarySerializerBuildInfo> factoryInfos = new();
        List<BinarySerializerBuildInfo> autoSerializers = new();

        foreach(BinarySerializerBuildInfo info in buildInfos)
        {
          if(info.TargetedType.IsGenericTypeDefinition)
          {
            if(info.DeclaringType != info.TargetedType) { continue; }
            _factories[info.TargetedType] = new BinaryConverterFactory(info.DeclaringType);
            _factories[info.TargetedType].AddTemplate(DefaultConverterName, info.DeclaringType);
            continue;
          }
          if(info.TargetedType.ContainsGenericParameters)
          {
            if(!info.DeclaringType.IsGenericTypeDefinition 
              || info.DeclaringType.GenericTypeArguments.Length != info.TargetedType.GenericTypeArguments.Length) 
            { 
              continue; 
            }
            Type defType = info.TargetedType.GetGenericTypeDefinition();
            _factories[defType] = new BinaryConverterFactory(defType);
            _factories[defType].AddTemplate(info.Name, defType);
            continue;
          }
          if(info.DeclaringType.IsGenericType && !info.DeclaringType.IsGenericTypeDefinition)
          {
            if(info.TargetedType.IsGenericType && !info.TargetedType.ContainsGenericParameters)
            {
              factoryInfos.Add(info);
            }
            continue;
          }

          if(info.Behavior != SerializationBehavior.Manual)
          {
            if(info.TargetedType.HasCreator(true))
            {
              autoTypes.Add(info.TargetedType);
              autoSerializers.Add(info);
            }
            else
            {
              Diagnostics.LogWarning(
                $"The Type '{info.TargetedType}' marked with the BinarySerializable Attribute does not have any available parameterless" +
                $" constructors or parameterless Creators. Unable to build a serialization schema."
              );
            }
            continue;
          }

          if(!TrySelectSerializerMethods(info.DeclaringType, info.TargetedType, out MethodInfo read, out MethodInfo write))
          {
            Diagnostics.LogWarning($"The Type '{info.DeclaringType.Name}' marked with a Binary Serialization Attribute for the Type '{info.TargetedType.Name}' " +
              $"is missing one or both of the required Read and Write methods. Read Missing: {read == null}, Write Missing: {write == null} ");
            continue;
          }
          BinaryConverter converter = (BinaryConverter)ManualConverterBuilder.MakeGenericMethod(info.TargetedType).Invoke(null, new object[] { info.Name, read, write });
          GetOrBuildCollection(info.TargetedType).AddConverter(converter);

        }

        foreach(BinarySerializerBuildInfo info in factoryInfos)
        {
          if(_factories.TryGetValue(info.TargetedType.GetGenericTypeDefinition(), out BinaryConverterFactory factory)
            && factory.TryBuild(info.TargetedType, out BinaryConverter converter, info.Name, autoTypes))
          {
            GetOrBuildCollection(info.TargetedType).AddConverter(converter);
          }
        }

        foreach(BinarySerializerBuildInfo info in autoSerializers)
        {
          BinaryConverter converter = (BinaryConverter)SchemaConverterBuilder.MakeGenericMethod(info.TargetedType).Invoke(null, new object[] { info.Name, info.Behavior, autoTypes });
          GetOrBuildCollection(info.TargetedType).AddConverter(converter);
        }

        _initialized = true;
      }
    }


    private static BinaryConverter BuildManualConverter<T>(string name, MethodInfo readInfo, MethodInfo writeInfo)
    {
      return new BinaryConverter<T>(
        name,
        DelegateBuilder.Func<BinaryReader, T>(readInfo),
        DelegateBuilder.Action<BinaryWriter, T>(writeInfo)
      );
    }

    private static BinaryConverter BuildSchemaConverter<T>(string name, SerializationBehavior behavior, HashSet<Type> autoTypes)
    {
      return new BinarySchemaConverter<T>(name, behavior, autoTypes);
    }

    private static bool TryBuildEnumerableConverter(Type type, out BinaryConverter converter)
    {
      lock(_enumerableLock)
      {
        if(_collections.TryGetValue(type, out BinaryConverterCollection collection))
        {
          return collection.TryGetConverter(out converter);
        }
        converter = null;
        Type innerType = null;
        if(type.IsArray)
        {
          innerType = type.GetElementType();
        }
        else
        {
          Type[] iEnums = type.GetEnumerableTypes().ToArray();
          if(iEnums.Length == 1)
          {
            innerType = iEnums[0].GenericTypeArguments[0];
          }
        }
        if(innerType != null && IsSerializable(innerType))
        {
          object resl = _enumerableBuilder.MakeGenericMethod(type, innerType).Invoke(null, new object[0]);
          if(resl != null)
          {
            collection = GetOrBuildCollection(type);
            converter = (BinaryConverter)resl;
            collection.AddConverter(converter);
          }
        }
        return converter != null;
      }
    }

    private static BinaryConverter BuildEnumerableConverter<T, TItem>() where T : IEnumerable<TItem>
    {
      if(!typeof(T).HasCreator(typeof(IEnumerable<TItem>))){ return null; }
      return new BinaryEnumerableConverter<T, TItem>(DefaultConverterName);
    }

    private static BinaryConverterCollection GetOrBuildCollection(Type type)
    {
      if(!_collections.TryGetValue(type, out BinaryConverterCollection collection))
      {
        _collections[type] = new(type);
        collection = _collections[type];
      }
      return collection;
    }

    private static bool TryBuildConverter(Type type, out BinaryConverter converter, bool buildEnumerable, string name)
    {
      if(!type.IsGenericType || type.ContainsGenericParameters) 
      {
        converter = null;
        return false; 
      }
      lock(_buildLock)
      {
        if(_collections.TryGetValue(type, out BinaryConverterCollection collection)
          && (name != null ? collection.TryGetConverter(name, out converter) : collection.TryGetConverter(out converter)))
        {
          return true;
        }
        if(_factories.TryGetValue(type.GetGenericTypeDefinition(), out BinaryConverterFactory factory))
        {
          if(factory.TryBuild(type, out converter, name))
          {
            GetOrBuildCollection(type).AddConverter(converter);
            return true;
          }
        }
        if(buildEnumerable)
        {
          return TryBuildEnumerableConverter(type, out converter);
        }
        converter = null;
        return false;
      }
    }

    private static void InitializeSystemTypes()
    {
      _collections[typeof(bool)] = new(typeof(bool));
      _collections[typeof(bool)].AddConverter(new BinaryConverter<bool>(DefaultConverterName, r => r.ReadBoolean(), (w, v) => w.Write(v)));
      _collections[typeof(byte)] = new(typeof(byte));
      _collections[typeof(byte)].AddConverter(new BinaryConverter<byte>(DefaultConverterName, r => r.ReadByte(), (w, v) => w.Write(v)));
      _collections[typeof(sbyte)] = new(typeof(sbyte));
      _collections[typeof(sbyte)].AddConverter(new BinaryConverter<sbyte>(DefaultConverterName, r => r.ReadSByte(), (w, v) => w.Write(v)));
      _collections[typeof(short)] = new(typeof(short));
      _collections[typeof(short)].AddConverter(new BinaryConverter<short>(DefaultConverterName, r => r.ReadInt16(), (w, v) => w.Write(v)));
      _collections[typeof(ushort)] = new(typeof(ushort));
      _collections[typeof(ushort)].AddConverter(new BinaryConverter<ushort>(DefaultConverterName, r => r.ReadUInt16(), (w, v) => w.Write(v)));
      _collections[typeof(int)] = new(typeof(int));
      _collections[typeof(int)].AddConverter(new BinaryConverter<int>(DefaultConverterName, r => r.ReadInt32(), (w, v) => w.Write(v)));
      _collections[typeof(uint)] = new(typeof(uint));
      _collections[typeof(uint)].AddConverter(new BinaryConverter<uint>(DefaultConverterName, r => r.ReadUInt32(), (w, v) => w.Write(v)));
      _collections[typeof(long)] = new(typeof(long));
      _collections[typeof(long)].AddConverter(new BinaryConverter<long>(DefaultConverterName, r => r.ReadInt64(), (w, v) => w.Write(v)));
      _collections[typeof(ulong)] = new(typeof(ulong));
      _collections[typeof(ulong)].AddConverter(new BinaryConverter<ulong>(DefaultConverterName, r => r.ReadUInt64(), (w, v) => w.Write(v)));
      _collections[typeof(float)] = new(typeof(float));
      _collections[typeof(float)].AddConverter(new BinaryConverter<float>(DefaultConverterName, r => r.ReadSingle(), (w, v) => w.Write(v)));
      _collections[typeof(double)] = new(typeof(double));
      _collections[typeof(double)].AddConverter(new BinaryConverter<double>(DefaultConverterName, r => r.ReadDouble(), (w, v) => w.Write(v)));
      _collections[typeof(decimal)] = new(typeof(decimal));
      _collections[typeof(decimal)].AddConverter(new BinaryConverter<decimal>(DefaultConverterName, r => r.ReadDecimal(), (w, v) => w.Write(v)));
      _collections[typeof(char)] = new(typeof(char));
      _collections[typeof(char)].AddConverter(new BinaryConverter<char>(DefaultConverterName, r => r.ReadChar(), (w, v) => w.Write(v)));
      _collections[typeof(string)] = new(typeof(string));
      _collections[typeof(string)].AddConverter(new BinaryConverter<string>(DefaultConverterName, r => r.ReadString(), (w, v) => w.Write(v)));
    }

  }
}
