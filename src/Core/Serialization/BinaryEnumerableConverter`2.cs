using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;


namespace WrathTools
{
  internal class BinaryEnumerableConverter<T, TItem> : BinaryConverter<T>
  {

    private enum EnumerableType
    {
      Array,
      Enumerable,
      Collection
    }

    private ICreator<IEnumerable<TItem>, T> _create;
    private int _arrayRank;
    private EnumerableType _enumerableType;
    private BinaryConverter<TItem> _innerConverter;


    public BinaryEnumerableConverter()
    {
      _innerConverter = BinarySerialization.GetConverter<TItem>();
      if(this.Type.IsArray)
      {
        _enumerableType = EnumerableType.Array;
        _arrayRank = this.Type.GetArrayRank();
      }
      else
      {
        _enumerableType = this.Type.GetInterfaces().Count(i => i == typeof(ICollection<TItem>)) > 0
          ? EnumerableType.Collection : EnumerableType.Enumerable;
        _create = Creators<IEnumerable<TItem>>.GetCreator<T>();
      }
    }

    private T ReadEnumerable(BinaryReader reader)
    {
      return _enumerableType == EnumerableType.Array
        ? PopulateArray(reader)
        : _create.Create(ReadLoop(reader));
    }

    private void WriteEnumerable(BinaryWriter writer, T instance)
    {
      switch(_enumerableType)
      {
        case EnumerableType.Array:
        {
          for(int i = 0; i < _arrayRank; i++)
          {
            writer.Write((instance as Array).GetLength(i));
          }
          break;
        }
        case EnumerableType.Enumerable:
        {
          writer.Write((instance as IEnumerable<TItem>).Count());
          break;
        }
        case EnumerableType.Collection:
        {
          writer.Write((instance as  ICollection<TItem>).Count);
          break;
        }
      }
      foreach(TItem item in instance as IEnumerable<TItem>)
      {
        _innerConverter.Write.Invoke(writer, item);
      }
    }

    private IEnumerable<TItem> ReadLoop(BinaryReader reader)
    {
      int count = reader.ReadInt32();
      for(int i = 0; i < count; i++)
      {
        yield return _innerConverter.Read.Invoke(reader);
      }
    }

    private T PopulateArray(BinaryReader reader)
    {
      int[] lengths = new int[_arrayRank];
      for(int i = 0; i < _arrayRank; i++)
      {
        lengths[i] = reader.ReadInt32();
      }
      switch(_arrayRank)
      {
        case 1:
        {
          TItem[] arr = new TItem[lengths[0]];
          for(int i = 0; i < arr.Length; i++)
          {
            arr[i] = _innerConverter.Read.Invoke(reader);
          }
          return (T)(object)arr;
        }
        case 2:
        {
          TItem[,] arr = new TItem[lengths[0], lengths[1]];
          for(int i1 = 0; i1 < lengths[0]; i1++)
          {
            for(int i2 = 0; i2 < lengths[1]; i2++)
            {
              arr[i1, i2] = _innerConverter.Read.Invoke(reader);
            }
          }
          return (T)(object)arr;
        }
        case 3:
        {
          TItem[,,] arr = new TItem[lengths[0], lengths[1], lengths[2]];
          for(int i1 = 0; i1 < lengths[0]; i1++)
          {
            for(int i2 = 0; i2 < lengths[1]; i2++)
            {
              for(int i3 = 0; i3 < lengths[2]; i3++)
              {
                arr[i1, i2, i3] = _innerConverter.Read.Invoke(reader);
              }
            }
          }
          return (T)(object)arr;
        }
        default:
        {
          Array arr = Array.CreateInstance(typeof(TItem), lengths);
          int[] curr = new int[_arrayRank];
          for(int d = _arrayRank - 1; d >= 0; d--)
          {
            for(int i = 0; i < lengths[d]; i++)
            {
              arr.SetValue(_innerConverter.Read.Invoke(reader), curr);
              curr[d]++;
            }
          }
          return (T)(object)arr;
        }
      }
    }

  }
}
