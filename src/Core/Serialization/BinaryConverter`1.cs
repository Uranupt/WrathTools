using System;
using System.IO;


namespace WrathTools
{
  public class BinaryConverter<T> : BinaryConverter
  {


    private Func<BinaryReadContext, T> _read;
    private Action<BinaryWriteContext, T> _write;
    public override Type Type => typeof(T);

    protected BinaryConverter(string name) : base(name)
    {
      
    }

    internal BinaryConverter(string name, Func<BinaryReadContext, T> read, Action<BinaryWriteContext, T> write) : base(name)
    {
      SetMethods(read, write);
    }

    public void Write(BinaryWriteContext context, T instance)
    {
      if(IsReferenceType)
      {
        context.WriteAsReference(instance, _write);
      }
      else
      {
        _write.Invoke(context, instance);
      }
    }

    public new T Read(BinaryReadContext context)
    {
      return IsReferenceType ? context.ReadAsReference(_read) : _read.Invoke(context);
    }

    protected void SetMethods(Func<BinaryReadContext, T> read, Action<BinaryWriteContext, T> write)
    {
      _read = read;
      _write = write;
      base.SetMethods(r => read.Invoke(r), (w, v) => write.Invoke(w, (T)v));
    }

  }
}
