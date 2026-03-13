using Mono.Cecil;
using System;
using System.Threading.Tasks;


namespace WrathTools
{
  public abstract class BinaryConverter
  {

    private Func<BinaryReadContext, object> _read;
    private Func<BinaryReadContext, Task<object>> _readAsync;
    private Action<BinaryWriteContext, object> _write;
    private Func<BinaryWriteContext, object, Task> _writeAsync;

    public readonly string Name;
    public virtual bool IsReferenceType { get; protected set; }
    public abstract Type Type { get; }

    protected BinaryConverter(string name)
    {
      Name = name;
      IsReferenceType = !Type.IsValueType && Type != typeof(string);
    }

    protected BinaryConverter(string name, Func<BinaryReadContext, object> read, Action<BinaryWriteContext, object> write)
    {
      Name = name;
      SetMethods(read, write);
    }

    public void Write(BinaryWriteContext context, object instance)
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

    public async Task WriteAsync(BinaryWriteContext context, object instance)
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

    public object Read(BinaryReadContext context)
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

    public async Task<object> ReadAsync(BinaryReadContext context)
    {
      if(_readAsync == null)
      {
        return Read(context);
      }
      else
      {
        return await (IsReferenceType ? context.ReadAsReferenceAsync(_readAsync) : _readAsync.Invoke(context));
      }
    }

    protected void SetMethods(Func<BinaryReadContext, object> read, Action<BinaryWriteContext, object> write)
    {
      _read = read;
      _write = write;
    }

    protected void SetAsyncMethods(Func<BinaryReadContext, Task<object>> read, Func<BinaryWriteContext, object, Task> write)
    {
      _readAsync = read;
      _writeAsync = write;
    }

  }
}
