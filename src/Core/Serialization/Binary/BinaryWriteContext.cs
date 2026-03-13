using System.IO;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;


namespace WrathTools
{
  public sealed class BinaryWriteContext : IDisposable
  {

    private readonly string _defaultSerializer;
    private readonly Dictionary<object, int> _graphIndexer = new(ReferenceComparer.Instance);

    public readonly BinaryWriter Writer;
    public bool IsDisposed { get; private set; }

    public BinaryWriteContext(BinaryWriter writer, string defaultSerializer = null)
    {
      Writer = writer;
      _defaultSerializer = defaultSerializer;
    }

    public BinaryWriteContext(Stream stream, string defaultSerializer = null)
    {
      Writer = new BinaryWriter(stream);
      _defaultSerializer = defaultSerializer;
    }

    public static implicit operator BinaryWriteContext(BinaryWriter writer) => new(writer);

    public void Dispose()
    {
      if(IsDisposed) { return; }
      Writer.Dispose();
      IsDisposed = true;
    }

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

    internal async Task WriteAsReferenceAsync<T>(T value, Func<BinaryWriteContext, T, Task> write)
    {
      if(_graphIndexer.TryGetValue(value, out int index))
      {
        Writer.Write(index);
        return;
      }
      index = _graphIndexer.Count;
      _graphIndexer[value] = index;
      Writer.Write(index);
      await write.Invoke(this, value);
    }

  }
}
