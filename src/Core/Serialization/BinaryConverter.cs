using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;


namespace WrathTools
{
  public class BinaryConverter
  {

    public readonly Func<BinaryReader, object> Read;
    public readonly Action<BinaryWriter, object> Write;

    internal static BinaryConverter BuildGeneric<T>(MethodInfo readInfo, MethodInfo writeInfo)
    {
      return new BinaryConverter<T>(
        DelegateBuilder.Func<BinaryReader, T>(readInfo),
        DelegateBuilder.Action<BinaryWriter, T>(writeInfo)
      );
    }

    internal BinaryConverter(Func<BinaryReader, object> read, Action<BinaryWriter, object> write)
    {
      Read = read;
      Write = write;
    }

  }
}
