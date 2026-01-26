using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;


namespace WrathTools
{
  public abstract class BinaryConverter
  {

    public readonly string Name;
    public Func<BinaryReader, object> Read { get; private set; }
    public Action<BinaryWriter, object> Write { get; private set; }
    public abstract Type Type { get; }

    protected BinaryConverter(string name)
    {
      Name = name;
    }

    protected BinaryConverter(string name, Func<BinaryReader, object> read, Action<BinaryWriter, object> write)
    {
      Name = name;
      SetMethods(read, write);
    }

    protected void SetMethods(Func<BinaryReader, object> read, Action<BinaryWriter, object> write)
    {
      Read = read;
      Write = write;
    }

  }
}
