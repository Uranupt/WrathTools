using System;
using System.Collections.Generic;
using System.IO;


namespace WrathTools
{
  internal class BinaryArraySerializer<T> : BinaryEnumerableSerializer
  {

    private Func<BinaryReader, T> //TODO: This is the gap, need a way to cast reader to type
    
    protected object Read(BinaryReader reader)
    {
      int count = reader.ReadInt32();
      T[] resl = new T[count];
      for(int i = 0; i < count; i++)
      {
        resl[i] = _innerRead
      }
    }

  }
}