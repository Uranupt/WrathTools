using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;


namespace WrathTools
{
  public abstract class BinaryConverter
  {

    private Func<BinaryReadContext, object> _read;
    private Action<BinaryWriteContext, object> _write;

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
      if(IsReferenceType)
      {
        context.WriteAsReference(instance, _write);
      }
      else
      {
        _write.Invoke(context, instance);
      }
    }

    public object Read(BinaryReadContext context)
    {
      return IsReferenceType ? context.ReadAsReference(_read) : _read.Invoke(context);
    }

    protected void SetMethods(Func<BinaryReadContext, object> read, Action<BinaryWriteContext, object> write)
    {
      _read = read;
      _write = write;
    }

  }
}
