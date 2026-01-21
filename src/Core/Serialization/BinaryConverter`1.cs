using System;
using System.IO;


namespace WrathTools
{
  public class BinaryConverter<T> : BinaryConverter
  {

    public new readonly Func<BinaryReader, T> Read;
    public new readonly Action<BinaryWriter, T> Write;

    internal BinaryConverter(Func<BinaryReader, T> read, Action<BinaryWriter, T> write)
      : base(r => read.Invoke(r), (w, v) => write.Invoke(w, (T)v))
    {
      Read = read;
      Write = write;
    }

  }
}
