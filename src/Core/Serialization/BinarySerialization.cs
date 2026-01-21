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

    private class ConvertibleInfo
    {

      public Func<BinaryReader, object> Read;
      public Action<BinaryWriter, object> Write;

    }

    private static bool _initialized;

    private readonly static Dictionary<Type, ConvertibleInfo> _convertibles = new()
    {
      //bool
      [typeof(bool)] = new ConvertibleInfo() { Read = r => r.ReadBoolean(), Write = (w, v) => w.Write((bool)v) },
      //byte
      [typeof(byte)] = new ConvertibleInfo() { Read = r => r.ReadByte(), Write = (w, v) => w.Write((byte)v) },
      //sbyte
      [typeof(sbyte)] = new ConvertibleInfo() { Read = r => r.ReadSByte(), Write = (w, v) => w.Write((sbyte)v) },
      //short
      [typeof(short)] = new ConvertibleInfo() { Read = r => r.ReadInt16(), Write = (w, v) => w.Write((short)v) },
      //ushort
      [typeof(ushort)] = new ConvertibleInfo() { Read = r => r.ReadUInt16(), Write = (w, v) => w.Write((ushort)v) },
      //int
      [typeof(int)] = new ConvertibleInfo() { Read = r => r.ReadInt32(), Write = (w, v) => w.Write((int)v) },
      //uint
      [typeof(uint)] = new ConvertibleInfo() { Read = r => r.ReadUInt32(), Write = (w, v) => w.Write((uint)v) },
      //long
      [typeof(long)] = new ConvertibleInfo() { Read = r => r.ReadInt64(), Write = (w, v) => w.Write((long)v) },
      //ulong
      [typeof(ulong)] = new ConvertibleInfo() { Read = r => r.ReadUInt64(), Write = (w, v) => w.Write((ulong)v) },
      //float
      [typeof(float)] = new ConvertibleInfo() { Read = r => r.ReadSingle(), Write = (w, v) => w.Write((float)v) },
      //double
      [typeof(double)] = new ConvertibleInfo() { Read = r => r.ReadDouble(), Write = (w, v) => w.Write((double)v) },
      //decimal
      [typeof(decimal)] = new ConvertibleInfo() { Read = r => r.ReadDecimal(), Write = (w, v) => w.Write((decimal)v) },
      //char
      [typeof(char)] = new ConvertibleInfo() { Read = r => r.ReadChar(), Write = (w, v) => w.Write((char)v) },
      //string
      [typeof(string)] = new ConvertibleInfo() { Read = r => r.ReadString(), Write = (w, v) => w.Write((string)v) }
    };

    private static Dictionary<Type, ConvertibleInfo> Convertibles
    {
      get
      {
        Initialize();
        return _convertibles;
      }
    }

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

    public static bool IsSerializable(Type type) => Convertibles.ContainsKey(type);
    public static bool IsSerializable<T>() => IsSerializable(typeof(T));
    public static bool IsSerializable(object obj) => IsSerializable(obj.GetType());
    public static bool IsBinarySerializable(this Type type) => IsSerializable(type);
    public static bool IsBinarySerializable(this object obj) => IsSerializable(obj);

    public static bool TryGetWrite(Type type, out Action<BinaryWriter, object> write, bool allowEnumerable = false)
    {
      if(Convertibles.TryGetValue(type, out ConvertibleInfo value))
      {
        write = value.Write;
        return true;
      }
      if(allowEnumerable)
      {
        return BinaryEnumerableSerializer.TryGetWrite(type, out write);
      }
      write = null;
      return write != null;
    }

    public static bool TryGetWrite<T>(out Action<BinaryWriter, object> write) => TryGetWrite(typeof(T), out write);

    public static bool TryWriteAs<T>(this BinaryWriter writer, T value)
    {
      if(TryGetWrite(typeof(T), out Action<BinaryWriter, object> write))
      {
        write?.Invoke(writer, value);
        return true;
      }
      return false;
    }

    public static bool TryWriteAsRuntime(this BinaryWriter writer, object value)
    {
      if(TryGetWrite(value.GetType(), out Action<BinaryWriter, object> write))
      {
        write?.Invoke(writer, value);
        return true;
      }
      return false;
    }

    public static bool TryWriteAs(this BinaryWriter writer, Type type, object value)
    {
      if(value.GetType() != type) { return false; }
      if(TryGetWrite(type, out Action<BinaryWriter, object> write))
      {
        write?.Invoke(writer, value);
        return true;
      }
      return false;
    }

    public static bool TryGetRead(Type type, out Func<BinaryReader, object> read, bool allowEnumerable = false)
    {
      if(Convertibles.TryGetValue(type, out ConvertibleInfo info))
      {
        read = info.Read;
        return true;
      }
      if(allowEnumerable)
      {
        return BinaryEnumerableSerializer.TryGetRead(type, out read);
      }
      read = null;
      return false;
    }

    public static bool TryGetRead<T>(out Func<BinaryReader, object> read) => TryGetRead(typeof(T), out read);

    public static bool TryReadAs(this BinaryReader reader, Type type, out object value)
    {
      if(TryGetRead(type, out Func<BinaryReader, object> read))
      {
        value = read?.Invoke(reader);
        return true;
      }
      value = default;
      return false;
    }

    public static bool TryReadAs<T>(this BinaryReader reader, out T value)
    {
      if(TryGetRead(typeof(T), out Func<BinaryReader, object> read))
      {
        value = (T)read.Invoke(reader);
        return true;
      }
      value = default;
      return false;
    }

    public static Action<BinaryWriter, object> GetWrite(Type type)
    {
      if(!TryGetWrite(type, out Action<BinaryWriter, object> write))
      {
        Diagnostics.LogError(
          new Exception(FailedGetWriteMessage(type.Name)),
          stackTrace: new(true)
        );
      }
      return write;
    }

    public static Action<BinaryWriter, object> GetWrite<T>()
    {
      if(!TryGetWrite<T>(out Action<BinaryWriter, object> write))
      {
        Diagnostics.LogError(
          new Exception(FailedGetWriteMessage(typeof(T).Name)),
          stackTrace: new(true)
        );
      }
      return write;
    }

    public static void WriteAs(this BinaryWriter writer, Type type, object value)
    {
      if(!writer.TryWriteAs(type, value))
      {
        Diagnostics.LogError(
          new Exception(FailedWriteMessage(type.Name)),
          stackTrace: new(true)
        );
      }
    }

    public static void WriteAs<T>(this BinaryWriter writer, T value)
    {
      if(!writer.TryWriteAs(value))
      {
        Diagnostics.LogError(
          new Exception(FailedWriteMessage(typeof(T).Name)),
          stackTrace: new(true)
        );
      }
    }

    public static void WriteAsRuntime(this BinaryWriter writer, object value)
    {
      if(!writer.TryWriteAsRuntime(value))
      {
        Diagnostics.LogError(
          new Exception(FailedWriteMessage(value.GetType().Name)),
          stackTrace: new(true)
        );
      }
    }

    public static Func<BinaryReader, object> GetRead(Type type)
    {
      if(!TryGetRead(type, out Func<BinaryReader, object> read))
      {
        Diagnostics.LogError(
          new Exception(FailedGetReadMessage(type.Name)),
          stackTrace: new(true)
        );
      }
      return read;
    }

    public static Func<BinaryReader, object> GetRead<T>()
    {
      if(!TryGetRead<T>(out Func<BinaryReader, object> read))
      {
        Diagnostics.LogError(
          new Exception(FailedGetReadMessage(typeof(T).Name)),
          stackTrace: new(true)
        );
      }
      return read;
    }

    public static object ReadAs(this BinaryReader reader, Type type)
    {
      if(!reader.TryReadAs(type, out object value))
      {
        Diagnostics.LogError(
          new Exception(FailedReadMessage(type.Name)),
          stackTrace: new(true)
        );
      }
      return value;
    }

    public static T ReadAs<T>(this BinaryReader reader)
    {
      if(!reader.TryReadAs(out T value))
      {
        Diagnostics.LogError(
          new Exception(FailedReadMessage(typeof(T).Name)),
          stackTrace: new(true)
        );
      }
      return value;
    }

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
      List<(Type, bool, bool)> autoTypes = new();
      foreach((Type type, BinarySerializableAttribute attr) in types)
      {
        if(attr.Manual)
        {
          manualTypes.Add(type);
        }
        else if(type.HasCreator() || type.GetConstructor(Type.EmptyTypes) != null)
        {
          autoTypes.Add((type, attr.SerializePublic, type.GetConstructor(Type.EmptyTypes) != null));
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

      foreach(Type type in manualTypes)
      {
        MethodInfo readInfo = null;
        MethodInfo writeInfo = null;
        foreach(MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public))
        {
          if(method.Name == "Read" && (method.ReturnType == type || method.ReturnType == typeof(object))
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
        Func<BinaryReader, object> read = (Func<BinaryReader, object>)Delegate.CreateDelegate(typeof(Func<BinaryReader, object>), readInfo);
        ParameterExpression valueParam = Expression.Parameter(typeof(object), "value");
        ParameterExpression writerParam = Expression.Parameter(typeof(object), "writer");
        UnaryExpression castValue = Expression.Convert(valueParam, type);
        MethodCallExpression call = Expression.Call(null, writeInfo, writerParam, castValue);
        Action<BinaryWriter, object> write = Expression.Lambda<Action<BinaryWriter, object>>(call, writerParam, valueParam).Compile();
        _convertibles[type] = new ConvertibleInfo() { Read = read, Write = write };
      }

      foreach((Type type, bool incPublic, bool canNew) in autoTypes)
      {
        BinarySerializationSchema schema = new(type, incPublic, canNew, allTypes);
        _convertibles[type] = new ConvertibleInfo() { Read = schema.Read, Write = schema.Write };
      }
    }

    private static string FailedGetWriteMessage(string typeName) => $"Failed to find the Binary Write method for the Type '{typeName}'";
    private static string FailedWriteMessage(string typeName) => $"Failed to write to BinaryWriter as Type '{typeName}'";
    private static string FailedGetReadMessage(string typeName) => $"Failed to find the Binary Read method for the Type '{typeName}'";
    private static string FailedReadMessage(string typeName) => $"Failed to read from the BinaryReader as Type '{typeName}'";

  }
}
