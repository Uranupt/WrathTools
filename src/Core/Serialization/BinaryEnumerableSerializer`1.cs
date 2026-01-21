using System;
using System.Collections.Generic;
using System.IO;


namespace WrathTools
{
  internal class BinaryEnumerableSerializer<T> : BinaryEnumerableSerializer
  {

    private Func<BinaryReader, T> _uncastRead;//TODO: This is the gap, need a way to cast reader to type

    private BinaryEnumerableSerializer(Func<object, int> getCount,
      Func<BinaryReader, T> innerRead, Action<BinaryWriter, object> innerWrite)
    {
      _getCount = getCount;
      _innerRead = innerRead;
      _innerWrite = innerWrite;
    }

    public override object Read(BinaryReader reader)
    {

    }

    private IEnumerable<T> ReadLoop(BinaryReader reader)
    {
      int count = reader.ReadInt32();
      for(int i = 0; i < count; i++)
      {
        yield return _uncastRead.Invoke(reader);
      }
    }

  }
}