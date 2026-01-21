using System;
using System.IO;


namespace WrathTools
{
  internal class BinaryConverter<T> : BinaryConverter
  {

    public override Func<BinaryReader, object> Read => r => ReadExact.Invoke(r); 
    public Func<BinaryReader, T> ReadExact { get; private set; }

    public

  }
}
