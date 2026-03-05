using System.IO;
using System.Collections.Generic;
using System;


namespace WrathTools
{
  public sealed class BinaryWriteContext
  {

    private readonly string _defaultSerializer;
    private readonly Dictionary<object, int> _graphIndexer = new(ReferenceComparer.Instance);

    public readonly BinaryWriter Writer;

    public BinaryWriteContext(BinaryWriter writer, string defaultSerializer = null)
    {
      Writer = writer;
      _defaultSerializer = defaultSerializer;
    }

    public static implicit operator BinaryWriteContext(BinaryWriter writer) => new(writer);

    internal void WriteAsReference<T>(T value, Action<BinaryWriteContext, T> write)
    {
      if(_graphIndexer.TryGetValue(value, out int index))
      {
        Writer.Write(index);
        return;
      }
      index = _graphIndexer.Count;
      _graphIndexer[value] = index;
      Writer.Write(index);
      write.Invoke(this, value);
    }

  }
}
