using System;
using System.IO;
using System.Xml.Linq;


namespace WrathTools
{
  public static partial class BinarySerialization
  {

    public static bool TryGetConverter(Type type, out BinaryConverter converter, bool buildIfEnumerable = true, string name = null)
    {
      if(Collections.TryGetValue(type, out BinaryConverterCollection collection))
      {
        if(name != null ? collection.TryGetConverter(name, out converter) : collection.TryGetConverter(out converter))
        {
          return true;
        }
      }
      return TryBuildConverter(type, out converter, buildIfEnumerable, name);
    }

    public static bool TryGetConverter<T>(out BinaryConverter<T> converter, bool buildIfEnumerable = true, string name = null)
    {
      converter = TryGetConverter(typeof(T), out BinaryConverter cvrt, buildIfEnumerable)
        ? (BinaryConverter<T>)cvrt : null;
      return converter != null;
    }

    public static bool IsSerializable(Type type, bool buildIfEnumerable = true, string name = null)
      => TryGetConverter(type, out _, buildIfEnumerable, name);

    public static bool IsSerializable<T>(bool buildIfEnumerable = true, string name = null) 
      => TryGetConverter(typeof(T), out _, buildIfEnumerable, name);

    public static bool IsSerializable(object obj, bool buildIfEnumerable = true, string name = null) 
      => TryGetConverter(obj.GetType(), out _, buildIfEnumerable, name);

    public static bool IsBinarySerializable(this Type type, bool buildIfEnumerable = true, string name = null) 
      => TryGetConverter(type, out _, buildIfEnumerable, name);

    public static bool IsBinarySerializable(this object obj, bool buildIfEnumerable = true, string name = null) 
      => TryGetConverter(obj.GetType(), out _, buildIfEnumerable, name);

    public static bool TryWriteAs(this BinaryWriter writer, Type type, object value, bool confirmRuntimeType = true, 
      bool buildIfEnumerable = true, string name = null)
    {
      if(confirmRuntimeType && value.GetType() != type) { return false; }
      if(TryGetConverter(type, out BinaryConverter converter, buildIfEnumerable, name))
      {
        converter.Write.Invoke(writer, value);
        return true;
      }
      return false;
    }

    public static bool TryWriteAs<T>(this BinaryWriter writer, T value, bool buildIfEnumerable = true, string name = null)
    {
      if(TryGetConverter<T>(out BinaryConverter<T> converter, buildIfEnumerable, name))
      {
        converter.Write.Invoke(writer, value);
        return true;
      }
      return false;
    }

    public static bool TryWriteAsRuntime(this BinaryWriter writer, object value, bool buildIfEnumerable = true, string name = null)
      => TryWriteAs(writer, value.GetType(), value, false, buildIfEnumerable, name);

    public static bool TryReadAs(this BinaryReader reader, Type type, out object value, bool buildIfEnumerable = true, string name = null)
    {
      if(TryGetConverter(type, out BinaryConverter converter, buildIfEnumerable, name))
      {
        value = converter.Read.Invoke(reader);
        return true;
      }
      value = default;
      return false;
    }

    public static bool TryReadAs<T>(this BinaryReader reader, out T value, bool buildIfEnumerable = true, string name = null)
    {
      if(TryGetConverter<T>(out BinaryConverter<T> converter, buildIfEnumerable, name))
      {
        value = converter.Read.Invoke(reader);
        return true;
      }
      value = default;
      return false;
    }

    public static BinaryConverter GetConverter(Type type, bool buildIfEnumerable = true, string name = null)
    {
      if(!TryGetConverter(type, out BinaryConverter converter, buildIfEnumerable, name))
      {
        Diagnostics.LogError(
          new Exception($"Failed to find a BinaryConverter for the Type '{type.Name}'." +
          $" Name supplied: '{name ?? "None"}', Allowed to Build Enumerable Converter: {buildIfEnumerable} "),
          stackTrace: new(true)
        );
      }
      return converter;
    }

    public static BinaryConverter<T> GetConverter<T>(bool buildIfEnumerable = true, string name = null) 
      => (BinaryConverter<T>)GetConverter(typeof(T), buildIfEnumerable, name);

    public static BinaryConverter GetBinaryConverter(this Type type, bool buildIfEnumerable = true, string name = null) 
      => GetConverter(type, buildIfEnumerable, name);

    public static void WriteAs(this BinaryWriter writer, Type type, object value, bool buildIfEnumerable = true, string name = null) 
      => GetConverter(type, buildIfEnumerable, name).Write.Invoke(writer, value);

    public static void WriteAs<T>(this BinaryWriter writer, T value, bool buildIfEnumerable = true, string name = null) 
      => GetConverter<T>(buildIfEnumerable, name).Write.Invoke(writer, value);

    public static void WriteAsRuntime(this BinaryWriter writer, object value, bool buildIfEnumerable = true, string name = null) 
      => WriteAs(writer, value.GetType(), value, buildIfEnumerable, name);

    public static object ReadAs(this BinaryReader reader, Type type, bool buildIfEnumerable = true, string name = null) 
      => GetConverter(type, buildIfEnumerable, name).Read.Invoke(reader);

    public static T ReadAs<T>(this BinaryReader reader, bool buildIfEnumerable = true, string name = null) 
      => GetConverter<T>(buildIfEnumerable, name).Read.Invoke(reader);

  }
}
