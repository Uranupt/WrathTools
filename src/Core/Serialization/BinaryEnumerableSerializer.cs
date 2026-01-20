using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Linq.Expressions;
using System.Collections;


namespace WrathTools
{
  internal class BinaryEnumerableSerializer
  {

    private readonly static Func<Type, bool> _enumerablePredicate = i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>);

    private readonly static Dictionary<Type, BinaryEnumerableSerializer> _serializers = new();

    private readonly Func<object, int> _getCount;
    private readonly Func<BinaryReader, object> _innerRead;
    private readonly Action<BinaryWriter, object> _innerWrite;

    public static bool TryGet(Type type, out BinaryEnumerableSerializer serializer)
    {
      if(_serializers.TryGetValue(type, out serializer)) { return true; }

      if(type.IsArray)
      {
        return TryBuildArray(type, out serializer);
      }
      else
      {
        Type iEnum = type.GetInterfaces().FirstOrDefault(_enumerablePredicate);
        if(iEnum == null) { return false; }

        Type iColl = type.GetInterfaces()
          .FirstOrDefault(i => i.IsGenericType
              && i.GetGenericTypeDefinition() == typeof(ICollection<>)
              && i.GenericTypeArguments[0] == iEnum.GenericTypeArguments[0]
          );

        return TryBuildEnumerable(type, iEnum, iColl, out serializer);
      }
    }

    private static bool TryGetMethods(Type type, out Func<BinaryReader, object> read, out Action<BinaryWriter, object> write)
    {
      read = null;
      write = null;
      if(type.IsArray || type.GetInterfaces().FirstOrDefault(_enumerablePredicate) != null)
      {
        if(TryGet(type, out BinaryEnumerableSerializer serializer))
        {
          read = serializer.Read;
          write = serializer.Write;
        }
      }
      else
      {
        BinarySerialization.TryGetRead(type, out read);
        BinarySerialization.TryGetWrite(type, out write);
      }
      return read != null && write != null;
    }

    private static Func<object, int> GetCountFunc(Type iColl)
    {
      ParameterExpression valueParam = Expression.Parameter(typeof(object), "value");
      UnaryExpression cast = Expression.Convert(valueParam, iColl);
      MemberExpression count = Expression.Property(cast, "Count");
      return Expression.Lambda<Func<object, int>>(count, valueParam).Compile();
    }

    private static int CountEnumerable(object enumerable)
    {
      int i = 0;
      IEnumerator enumerator = (enumerable as IEnumerable).GetEnumerator();
      while(enumerator.MoveNext())
      {
        i++;
      }
      return i;
    }

    private static bool TryBuildArray(Type type, out BinaryEnumerableSerializer serializer)
    {
      serializer = null;
      Type containedType = type.GetElementType();
      if(!TryGetMethods(containedType, out Func<BinaryReader, object> read, out Action<BinaryWriter, object> write))
      {
        return false;
      }

      _serializers[type] = new BinaryEnumerableSerializer(a => (a as Array).Length, read, write);
      serializer = _serializers[type];
      return true;
    }

    private static bool TryBuildEnumerable(Type type, Type iEnum, Type iColl, out BinaryEnumerableSerializer serializer)
    {
      serializer = null;
      Type containedType = iEnum.GenericTypeArguments[0];
      if(!TryGetMethods(containedType, out Func<BinaryReader, object> read, out Action<BinaryWriter, object> write))
      {
        return false;
      }
      Func<object, int> getCount = iColl != null ? GetCountFunc(iColl) : CountEnumerable;
      _serializers[type] = new BinaryEnumerableSerializer(getCount, read, write);
      serializer = _serializers[type];
      return true;
    }

    private BinaryEnumerableSerializer(Func<object, int> getCount, 
      Func<BinaryReader, object> innerRead, Action<BinaryWriter, object> innerWrite)
    {
      _getCount = getCount;
      _innerRead = innerRead;
      _innerWrite = innerWrite;
    }

    public object Read(BinaryReader reader)
    {
      int count = reader.ReadInt32();
      object[] arr = new object[count];
      for(int i = 0; i < count; i++)
      {
        arr[i] = _innerRead?.Invoke(reader);
      }
      return arr;
    }

    public void Write(BinaryWriter writer, object value)
    {
      int count = _getCount.Invoke(value);
      writer.Write(count);
      foreach(object obj in (IEnumerable)value)
      {
        _innerWrite?.Invoke(writer, obj);
      }
    }

  }
}
