using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace WrathTools
{
  public static class BinarySerialization
  {

    private static bool _initialized;
    private static MethodInfo _enumerableBuilder = typeof(BinarySerialization).GetMethod("BuildEnumerableConverter", BindingFlags.Static);

    private static readonly Dictionary<Type, BinaryConverter> _converters = new()
    {
      [typeof(bool)] = new BinaryConverter<bool>(r => r.ReadBoolean(), (w, v) => w.Write(v)),
      [typeof(byte)] = new BinaryConverter<byte>(r => r.ReadByte(), (w, v) => w.Write(v)),
      [typeof(sbyte)] = new BinaryConverter<sbyte>(r => r.ReadSByte(), (w, v) => w.Write(v)),
      [typeof(short)] = new BinaryConverter<short>(r => r.ReadInt16(), (w, v) => w.Write(v)),
      [typeof(ushort)] = new BinaryConverter<ushort>(r => r.ReadUInt16(), (w, v) => w.Write(v)),
      [typeof(int)] = new BinaryConverter<int>(r => r.ReadInt32(), (w, v) => w.Write(v)),
      [typeof(uint)] = new BinaryConverter<uint>(r => r.ReadUInt32(), (w, v) => w.Write(v)),
      [typeof(long)] = new BinaryConverter<long>(r => r.ReadInt64(), (w, v) => w.Write(v)),
      [typeof(ulong)] = new BinaryConverter<ulong>(r => r.ReadUInt64(), (w, v) => w.Write(v)),
      [typeof(float)] = new BinaryConverter<float>(r => r.ReadSingle(), (w, v) => w.Write(v)),
      [typeof(double)] = new BinaryConverter<double>(r => r.ReadDouble(), (w, v) => w.Write(v)),
      [typeof(decimal)] = new BinaryConverter<decimal>(r => r.ReadDecimal(), (w, v) => w.Write(v)),
      [typeof(char)] = new BinaryConverter<char>(r => r.ReadChar(), (w, v) => w.Write(v)),
      [typeof(string)] = new BinaryConverter<string>(r => r.ReadString(), (w, v) => w.Write(v))
    };

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

    internal static Dictionary<Type, BinaryConverter> Converters
    {
      get
      {
        Initialize();
        return _converters;
      }
    }

    public static bool IsSerializable(Type type, bool includeEnumerable = false) => TryGetConverter(type, out _, includeEnumerable);
    public static bool IsSerializable<T>(bool includeEnumerable = false) => IsSerializable(typeof(T), includeEnumerable);
    public static bool IsSerializable(object obj, bool includeEnumerable = false) => IsSerializable(obj.GetType(), includeEnumerable);
    public static bool IsBinarySerializable(this Type type, bool includeEnumerable = false) => IsSerializable(type, includeEnumerable);
    public static bool IsBinarySerializable(this object obj, bool includeEnumerable = false) => IsSerializable(obj, includeEnumerable);


    public static bool TryGetConverter(this Type type, out BinaryConverter converter, bool includeEnumerable)
    {
      if(!Converters.TryGetValue(type, out converter))
      {
        if(includeEnumerable)
        {
          TryBuildEnumerableConverter(type, out converter);
        }
      }
      return converter != null;
    }

    public static bool TryGetConverter<T>(out BinaryConverter<T> converter, bool includeEnumerable = false)
    {
      converter = TryGetConverter(typeof(T), out BinaryConverter cvrt, includeEnumerable)
        ? (BinaryConverter<T>)cvrt : null;
      return converter != null;
    }

    public static bool TryGetWrite(Type type, out Action<BinaryWriter, object> write, bool includeEnumerable = false)
    {
      write = TryGetConverter(type, out BinaryConverter converter, includeEnumerable)
        ? converter.Write : null;
      return write != null;
    }

    public static bool TryGetWrite<T>(out Action<BinaryWriter, T> write, bool includeEnumerable = false)
    {
      write = TryGetConverter(out BinaryConverter<T> converter, includeEnumerable)
        ? converter.Write : null;
      return write != null;
    }

    public static bool TryGetRead(Type type, out Func<BinaryReader, object> read, bool includeEnumerable = false)
    {
      read = TryGetConverter(type, out BinaryConverter converter, includeEnumerable)
        ? converter.Read : null;
      return read != null;
    }

    public static bool TryGetRead<T>(out Func<BinaryReader, T> read, bool includeEnumerable = false)
    {
      read = TryGetConverter(out BinaryConverter<T> converter, includeEnumerable)
        ? converter.Read : null;
      return read != null;
    }

    public static bool TryWriteAs(this BinaryWriter writer, Type type, object value, bool runtimeCheck, bool includeEnumerable = false)
    {
      if(runtimeCheck && value.GetType() != type) { return false; }
      if(TryGetWrite(type, out Action<BinaryWriter, object> write, includeEnumerable))
      {
        write?.Invoke(writer, value);
        return true;
      }
      return false;
    }

    public static bool TryWriteAs<T>(this BinaryWriter writer, T value, bool includeEnumerable = false)
    {
      if(TryGetWrite(out Action<BinaryWriter, T> write, includeEnumerable))
      {
        write?.Invoke(writer, value);
        return true;
      }
      return false;
    }

    public static bool TryWriteAsRuntime(this BinaryWriter writer, object value, bool includeEnumerable = false)
      => TryWriteAs(writer, value.GetType(), false, includeEnumerable);

    public static bool TryReadAs(this BinaryReader reader, Type type, out object value, bool includeEnumerable = false)
    {
      if(TryGetRead(type, out Func<BinaryReader, object> read, includeEnumerable))
      {
        value = read?.Invoke(reader);
        return true;
      }
      value = default;
      return false;
    }

    public static bool TryReadAs<T>(this BinaryReader reader, out T value, bool includeEnumerable = false)
    {
      if(TryGetRead(out Func<BinaryReader, T> read, includeEnumerable))
      {
        value = read.Invoke(reader);
        return true;
      }
      value = default;
      return false;
    }

    public static BinaryConverter GetConverter(Type type)
    {
      if(!TryGetConverter(type, out BinaryConverter converter, true))
      {
        Diagnostics.LogError(
          new Exception($"Failed to find a BinaryConverter for the Type '{type.Name}'"),
          stackTrace: new(true)
        );
      }
      return converter;
    }

    public static BinaryConverter<T> GetConverter<T>() => (BinaryConverter<T>)GetConverter(typeof(T));
    public static BinaryConverter GetBinaryConverter(this Type type) => GetConverter(type);

    public static Action<BinaryWriter, object> GetWrite(Type type) => GetConverter(type).Write;
    public static Action<BinaryWriter, T> GetWrite<T>() => GetConverter<T>().Write;

    public static void WriteAs(this BinaryWriter writer, Type type, object value) => GetWrite(type).Invoke(writer, value);
    public static void WriteAs<T>(this BinaryWriter writer, T value) => GetWrite<T>().Invoke(writer, value);
    public static void WriteAsRuntime(this BinaryWriter writer, object value) => WriteAs(writer, value.GetType(), value);

    public static Func<BinaryReader, object> GetRead(Type type) => GetConverter(type).Read;
    public static Func<BinaryReader, T> GetRead<T>() => GetConverter<T>().Read;

    public static object ReadAs(this BinaryReader reader, Type type) => GetRead(type).Invoke(reader);
    public static T ReadAs<T>(this BinaryReader reader) => GetRead<T>().Invoke(reader);

    private static void Initialize()
    {
      if(_initialized) { return; }
      _initialized = true;
      IEnumerable<(Type, BinarySerializableAttribute)> types = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a => a.GetTypes())
        .Select(t => (type: t, attr: (BinarySerializableAttribute)t.GetCustomAttributes(typeof(BinarySerializableAttribute)).FirstOrDefault()))
        .Where(p => p.type != null && p.type.IsSealed && p.attr != null);
      HashSet<Type> allTypes = new();
      List<Type> manualTypes = new();
      List<(Type, bool)> autoTypes = new();
      foreach((Type type, BinarySerializableAttribute attr) in types)
      {
        if(attr.Manual)
        {
          manualTypes.Add(type);
        }
        else if(type.HasCreator(true))
        {
          autoTypes.Add((type, attr.SerializePublic));
        }
        else
        {
          continue;
        }
        allTypes.Add(type);
      }

      static bool ParamCheck(ParameterInfo[] parameters, params Type[] paramTypes)
      {
        if(parameters.Length != paramTypes.Length) { return false; }
        for(int i = 0; i < parameters.Length; i++)
        {
          if(parameters[i].ParameterType !=  paramTypes[i]) { return false; }
        }
        return true;
      }

      MethodInfo buildConverter = typeof(BinarySerialization).GetMethod("BuildManualConverter", BindingFlags.Static);

      foreach(Type type in manualTypes)
      {
        MethodInfo readInfo = null;
        MethodInfo writeInfo = null;
        foreach(MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
        {
          if(method.Name == "Read" && method.ReturnType == type
            && ParamCheck(method.GetParameters(), typeof(BinaryReader)))
          {
            readInfo = method;
          }
          if(method.Name == "Write" && ParamCheck(method.GetParameters(), typeof(BinaryWriter), type))
          {
            writeInfo = method;
          }
        }
        if(readInfo == null || writeInfo == null)
        {
          Diagnostics.LogWarning($"The Type '{type.Name}' marked with the BinarySerializable Attribute is missing one or both of the required Read and Write methods." +
            $" \n Read Missing: {readInfo == null}, Write Missing: {writeInfo == null} ");
          allTypes.Remove(type);
          continue;
        }
        _converters[type] = buildConverter.MakeGenericMethod(type).Invoke(null, new object[] { readInfo, writeInfo }) as BinaryConverter;
      }

      MethodInfo schemaConverter = typeof(BinarySerialization).GetMethod("BuildSchemaConverter", BindingFlags.Static);

      foreach((Type type, bool incPublic) in autoTypes)
      {
        _converters[type] = schemaConverter.MakeGenericMethod(type).Invoke(null, new object[] { incPublic, allTypes }) as BinaryConverter;
      }
    }

    private static BinaryConverter BuildManualConverter<T>(MethodInfo readInfo, MethodInfo writeInfo)
    {
      return new BinaryConverter<T>(
        DelegateBuilder.Func<BinaryReader, T>(readInfo),
        DelegateBuilder.Action<BinaryWriter, T>(writeInfo)
      );
    }

    private static BinaryConverter BuildSchemaConverter<T>(bool incPublic, HashSet<Type> allowedTypes)
    {
      return new BinarySchemaConverter<T>(incPublic, allowedTypes);
    }

    private static bool TryBuildEnumerableConverter(Type type, out BinaryConverter converter)
    {
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

      converter = innerType != null && !IsSerializable(innerType, true)
        ? _enumerableBuilder.MakeGenericMethod(type, innerType).Invoke(null, new object[0]) as BinaryConverter
        : null;

      return converter != null;
    }

    private static BinaryConverter BuildEnumerableConverter<T, TItem>()
    {
      if(!Creators<IEnumerable<TItem>>.HasCreator<T>()) { return null; }
      return new BinaryEnumerableConverter<T, TItem>();
    }

  }
}
