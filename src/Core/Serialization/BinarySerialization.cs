using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace WrathTools
{
  public static class BinarySerialization
  {

    private class ConvertibleInfo
    {

      public Func<BinaryReader, object> Read;
      public Action<BinaryWriter, object> Write;

    }

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


    public static bool IsWritable(Type type)
    {
      if(_convertibles.TryGetValue(type, out ConvertibleInfo value))
      {
        return value.Write != null;
      }
      if(typeof(IBinaryWritable).IsAssignableFrom(type))
      {
        BuildInfo(type);
        return true;
      }
      return false;
    }

    public static bool IsWritable<T>() => IsWritable(typeof(T));
    public static bool IsWritable(object obj) => IsWritable(obj.GetType());
    public static bool IsBinaryWritable(this Type type) => IsWritable(type);
    public static bool IsBinaryWritable(this object obj) => IsWritable(obj);

    public static bool IsReadable(Type type)
    {
      if(_convertibles.TryGetValue(type, out ConvertibleInfo value))
      {
        return value.Read != null;
      }
      if(typeof(IBinaryReadable).IsAssignableFrom(type))
      {
        BuildInfo(type);
        return true;
      }
      return false;
    }

    public static bool IsReadable<T>() => IsReadable(typeof(T));
    public static bool IsReadable(object obj) => IsReadable(obj.GetType());
    public static bool IsBinaryReadable(this Type type) => IsReadable(type);
    public static bool IsBinaryReadable(this object obj) => IsReadable(obj);

    public static bool IsConvertible(Type type)
    {
      if(_convertibles.TryGetValue(type, out ConvertibleInfo value))
      {
        return value.Read != null && value.Write != null;
      }
      if(typeof(IBinaryConvertible).IsAssignableFrom(type)
        || (typeof(IBinaryReadable).IsAssignableFrom(type) && typeof(IBinaryWritable).IsAssignableFrom(type)))
      {
        BuildInfo(type);
        return true;
      }
      return false;
    }

    public static bool IsConvertible<T>() => IsConvertible(typeof(T));
    public static bool IsConvertible(object obj) => IsConvertible(obj.GetType());
    public static bool IsBinaryConvertible(this Type type) => IsConvertible(type);
    public static bool IsBinaryConvertible(this object obj) => IsConvertible(obj);

    public static bool TryGetWrite(Type type, out Action<BinaryWriter, object> write)
    {
      if(_convertibles.TryGetValue(type, out ConvertibleInfo value))
      {
        write = value.Write;
        return write != null;
      }
      else if(typeof(IBinaryWritable).IsAssignableFrom(type))
      {
        write = BuildInfo(type).Write;
        return true;
      }
      write = null;
      return false;
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

    public static bool TryGetRead(Type type, out Func<BinaryReader, object> read)
    {
      if(_convertibles.TryGetValue(type, out ConvertibleInfo info))
      {
        read = info.Read;
        return read != null;
      }
      else if(typeof(IBinaryReadable).IsAssignableFrom(type))
      {
        read = BuildInfo(type).Read;
        return true;
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

    private static ConvertibleInfo BuildInfo(Type type)
    {
      ConvertibleInfo info = new();
      if(typeof(IBinaryWritable).IsAssignableFrom(type))
      {
        info.Write = (w, v) => (v as IBinaryWritable).Write(w);
      }
      if(typeof(IBinaryReadable).IsAssignableFrom(type))
      {
        info.Read = (Activator.CreateInstance(type) as IBinaryReadable).GetReader();
      }
      _convertibles[type] = info;
      return info;
    }

    private static string FailedGetWriteMessage(string typeName) => $"Failed to find the Binary Write method for the Type '{typeName}'";
    private static string FailedWriteMessage(string typeName) => $"Failed to write to BinaryWriter as Type '{typeName}'";
    private static string FailedGetReadMessage(string typeName) => $"Failed to find the Binary Read method for the Type '{typeName}'";
    private static string FailedReadMessage(string typeName) => $"Failed to read from the BinaryReader as Type '{typeName}'";

  }
}
