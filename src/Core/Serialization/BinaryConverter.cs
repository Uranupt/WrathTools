using System;
using System.IO;


namespace WrathTools
{
  internal class BinaryConverter
  {

    public virtual Func<BinaryReader, object> Read { get; private set; }
    public virtual Action<BinaryWriter, object> Write { get; private set; }

    public BinaryConverter(Func<BinaryReader, object> read, Action<BinaryWriter, object> write)
    {
      Read = read;
      Write = write;
    }

  }
}
