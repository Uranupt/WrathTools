using Mono.Cecil;
using System;
using System.Threading.Tasks;


namespace WrathTools
{
  public class BinaryConverter<T> : BinaryConverter
  {

    private Func<BinaryReadContext, T> _read;
    private Func<BinaryReadContext, Task<T>> _readAsync;
    private Action<BinaryWriteContext, T> _write;
    private Func<BinaryWriteContext, T, Task> _writeAsync;
    public override Type Type => typeof(T);

    protected BinaryConverter(string name) : base(name)
    {
      
    }

    internal BinaryConverter(string name, Func<BinaryReadContext, T> read, Action<BinaryWriteContext, T> write) : base(name)
    {
      SetMethods(read, write);
    }

    internal BinaryConverter(string name, Func<BinaryReadContext, Task<T>> readAsync, Func<BinaryWriteContext, T, Task> writeAsync)
      : base(name)
    {
      SetAsyncMethods(readAsync, writeAsync);
    }

    internal BinaryConverter(string name, Func<BinaryReadContext, T> read, Action<BinaryWriteContext, T> write,
      Func<BinaryReadContext, Task<T>> readAsync, Func<BinaryWriteContext, T, Task> writeAsync)
      :base(name)
    {
      SetMethods(read, write);
      SetAsyncMethods(readAsync, writeAsync);
    }

    public void Write(BinaryWriteContext context, T instance)
    {
      if(_write == null)
      {
        if(_writeAsync == null)
        {
          Diagnostics.LogError(
            new Exception("BinaryConverter has neither a valid Write nor WriteAsync method."),
            stackTrace: new(true),
            id: Serialization.DiagnosticID + ".missing_write_methods.binary"
          );
        }
        else
        {
          WriteAsync(context, instance).GetAwaiter().GetResult();
        }
        return;
      }
      if(IsReferenceType)
      {
        context.WriteAsReference(instance, _write);
      }
      else
      {
        _write.Invoke(context, instance);
      }
    }

    public async Task WriteAsync(BinaryWriteContext context, T instance)
    {
      if(_writeAsync == null)
      {
        Write(context, instance);
      }
      else
      {
        await (IsReferenceType ? context.WriteAsReferenceAsync(instance, _writeAsync) : _writeAsync.Invoke(context, instance));
      }
    }

    public new T Read(BinaryReadContext context)
    {
      if(_read == null)
      {
        if(_readAsync == null)
        {
          Diagnostics.LogError(
            new Exception("BinaryConverter has neither a valid Read nor ReadAsync method."),
            stackTrace: new(true),
            id: Serialization.DiagnosticID + ".missing_read_methods.binary"
          );
          return default;
        }
        else
        {
          return ReadAsync(context).GetAwaiter().GetResult();
        }
      }
      return IsReferenceType ? context.ReadAsReference(_read) : _read.Invoke(context);
    }

    public new async Task<T> ReadAsync(BinaryReadContext context)
    {
      if(_readAsync == null)
      {
        return Read(context);
      }
      return await (IsReferenceType ? context.ReadAsReferenceAsync(_readAsync) : _readAsync.Invoke(context));
    }

    protected void SetMethods(Func<BinaryReadContext, T> read, Action<BinaryWriteContext, T> write)
    {
      _read = read;
      _write = write;
      base.SetMethods(ReadWithCast, WriteWithCast);
    }

    protected void SetAsyncMethods(Func<BinaryReadContext, Task<T>> read, Func<BinaryWriteContext, T, Task> write)
    {
      _readAsync = read;
      _writeAsync = write;
      base.SetAsyncMethods(ReadWithCastAsync, WriteWithCastAsync);
    }

    private void WriteWithCast(BinaryWriteContext context, object value) => _write.Invoke(context, (T)value);

    private object ReadWithCast(BinaryReadContext context) => _read.Invoke(context);

    private async Task WriteWithCastAsync(BinaryWriteContext context, object value) => await _writeAsync.Invoke(context, (T)value);

    private async Task<object> ReadWithCastAsync(BinaryReadContext context) => await _readAsync.Invoke(context);

  }
}
