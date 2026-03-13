using System.IO;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;


namespace WrathTools
{
  public sealed class BinaryReadContext : IDisposable
  {

    private readonly string _defaultSerializer;
    private readonly List<object> _graphIndexer = new();
    private readonly Stack<int> _workingIndices = new();
    private readonly object _placeholder = new();

    public readonly BinaryReader Reader;
    public bool IsDisposed { get; private set; } = false;

    public BinaryReadContext(BinaryReader reader, string defaultSerializer = null)
    {
      Reader = reader;
      _defaultSerializer = defaultSerializer;
    }

    public BinaryReadContext(Stream stream, string defaultSerializer = null)
    {
      Reader = new BinaryReader(stream);
      _defaultSerializer = defaultSerializer;
    }

    public static implicit operator BinaryReadContext(BinaryReader reader) => new(reader);

    public void AddToGraph(object obj)
    {
      if(_workingIndices.Count == 0) { return; }
      _graphIndexer[_workingIndices.Pop()] = obj;
    }

    public void Dispose()
    {
      if(IsDisposed) { return; }
      Reader.Dispose();
      IsDisposed = true;
    }

    internal T ReadAsReference<T>(Func<BinaryReadContext, T> read)
    {
      int index = Reader.ReadInt32();
      if(index < _graphIndexer.Count)
      {
        if(_graphIndexer[index] == _placeholder)
        {
          Diagnostics.LogError(
            new Exception($"Attempted to fetch an object reference of Type '{typeof(T).Name}' which was in the process of being read and has" +
            $" not been added to the graph yet. Ensure 'BinaryReadContext.AddToGraph(object)' is called before attempting to read cyclical references."),
            stackTrace: new(true),
            id: $"{Serialization.DiagnosticID}.read_cyclical_reference_not_added.binary"
          );
        }
        return (T)_graphIndexer[index];
      }

      if(index > _graphIndexer.Count)
      {
        Diagnostics.LogError(
          new Exception($"Attempted to read a new object of Type '{typeof(T).Name}' with expected index of '{index}', " +
          $"but the current size of the graph is '{_graphIndexer.Count}'. Deserialization cannot continue."),
          stackTrace: new(true),
          id: $"{Serialization.DiagnosticID}.read_index_out_of_order.binary"
        );
        return default;
      }

      _graphIndexer.Add(_placeholder);
      _workingIndices.Push(index);
      T instance = read.Invoke(this);
      if(_workingIndices.TryPeek(out int top) && top == index)
      {
        AddToGraph(instance);
      }
      return instance;
    }

    internal async Task<T> ReadAsReferenceAsync<T>(Func<BinaryReadContext, Task<T>> read)
    {
      int index = Reader.ReadInt32();
      if(index < _graphIndexer.Count)
      {
        if(_graphIndexer[index] == _placeholder)
        {
          Diagnostics.LogError(
            new Exception($"Attempted to fetch an object reference of Type '{typeof(T).Name}' which was in the process of being read and has" +
            $" not been added to the graph yet. Ensure 'BinaryReadContext.AddToGraph(object)' is called before attempting to read cyclical references."),
            stackTrace: new(true),
            id: $"{Serialization.DiagnosticID}.read_cyclical_reference_not_added.binary"
          );
        }
        return (T)_graphIndexer[index];
      }

      if(index > _graphIndexer.Count)
      {
        Diagnostics.LogError(
          new Exception($"Attempted to read a new object of Type '{typeof(T).Name}' with expected index of '{index}', " +
          $"but the current size of the graph is '{_graphIndexer.Count}'. Deserialization cannot continue."),
          stackTrace: new(true),
          id: $"{Serialization.DiagnosticID}.read_index_out_of_order.binary"
        );
        return default;
      }

      _graphIndexer.Add(_placeholder);
      _workingIndices.Push(index);
      return await read.Invoke(this);
    }

  }
}
