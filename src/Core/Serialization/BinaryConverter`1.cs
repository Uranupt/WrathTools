using System;
using System.IO;


namespace WrathTools
{
  public class BinaryConverter<T> : BinaryConverter
  {


    public new Func<BinaryReader, T> Read { get; private set; }
    public new Action<BinaryWriter, T> Write { get; private set; }
    public override Type Type => typeof(T);

    protected BinaryConverter(string name) : base(name)
    {
      
    }

    internal BinaryConverter(string name, Func<BinaryReader, T> read, Action<BinaryWriter, T> write) : base(name)
    {
      SetMethods(read, write);
    }

    protected void SetMethods(Func<BinaryReader, T> read, Action<BinaryWriter, T> write)
    {
      Read = read;
      Write = write;
      base.SetMethods(r => read.Invoke(r), (w, v) => write.Invoke(w, (T)v));
    }

  }
}
