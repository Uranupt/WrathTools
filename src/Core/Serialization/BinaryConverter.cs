using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;


namespace WrathTools
{
  public abstract class BinaryConverter
  {

    public Func<BinaryReader, object> Read { get; private set; }
    public Action<BinaryWriter, object> Write { get; private set; }
    public abstract Type Type { get; }

    protected BinaryConverter()
    {

    }

    protected BinaryConverter(Func<BinaryReader, object> read, Action<BinaryWriter, object> write)
    {
      SetMethods(read, write);
    }

    protected void SetMethods(Func<BinaryReader, object> read, Action<BinaryWriter, object> write)
    {
      Read = read;
      Write = write;
    }

  }
}
