using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;


namespace WrathTools
{
  internal sealed class BinaryEnumerableConverter<T, TItem> : BinaryConverter<T> where T: IEnumerable<TItem>
  {

    private Creator<IEnumerable<TItem>, T> _creator;
    private int _arrayRank = 0;
    private BinaryConverter<TItem> _innerConverter;

    public override bool IsReferenceType => false;


    public BinaryEnumerableConverter(string name) : base(name)
    {
      _innerConverter = BinarySerialization.GetConverter<TItem>();
      if(this.Type.IsArray)
      {
        _arrayRank = this.Type.GetArrayRank();
      }
      else
      {
        _creator = (Creator<IEnumerable<TItem>, T>)typeof(T).GetCreator(typeof(IEnumerable<TItem>));
      }
      SetMethods(ReadEnumerable, WriteEnumerable);
    }

    private T ReadEnumerable(BinaryReadContext context)
    {
      return _arrayRank > 0
        ? PopulateArray(context)
        : _creator.Create(ReadLoop(context));
    }

    private void WriteEnumerable(BinaryWriteContext context, T instance)
    {
      if(_arrayRank > 0)
      {
        for(int i = 0; i < _arrayRank; i++)
        {
          context.Writer.Write((instance as Array).GetLength(i));
        }
      }
      else
      {
        context.Writer.Write(instance.Count());
      }
      foreach(TItem item in instance)
      {
        _innerConverter.Write(context, item);
      }
    }

    private IEnumerable<TItem> ReadLoop(BinaryReadContext context)
    {
      int count = context.Reader.ReadInt32();
      for(int i = 0; i < count; i++)
      {
        yield return _innerConverter.Read(context);
      }
    }

    private T PopulateArray(BinaryReadContext context)
    {
      int[] lengths = new int[_arrayRank];
      for(int i = 0; i < _arrayRank; i++)
      {
        lengths[i] = context.Reader.ReadInt32();
      }
      switch(_arrayRank)
      {
        case 1:
        {
          TItem[] arr = new TItem[lengths[0]];
          for(int i = 0; i < arr.Length; i++)
          {
            arr[i] = _innerConverter.Read(context);
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
              arr[i1, i2] = _innerConverter.Read(context);
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
                arr[i1, i2, i3] = _innerConverter.Read(context);
              }
            }
          }
          return (T)(object)arr;
        }
        default:
        {
          Array arr = Array.CreateInstance(typeof(TItem), lengths);
          int[] curr = new int[_arrayRank];
          int d = _arrayRank - 1;
          while(d >= 0)
          {
            for(int i = 0; i < lengths[^1]; i++)
            {
              arr.SetValue(_innerConverter.Read(context), curr);
              curr[^1]++;
            }
            while(d >= 0 && curr[d] >= lengths[d])
            {
              curr[d] = 0;
              d--;
            }
            if(d >= 0)
            {
              curr[d]++;
              d = _arrayRank - 1;
            }
          }
          return (T)(object)arr;
        }
      }
    }

  }
}
