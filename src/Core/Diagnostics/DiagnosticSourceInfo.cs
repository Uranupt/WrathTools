using System;
using System.Reflection;


namespace WrathTools
{
  public sealed class DiagnosticSourceInfo
  {

    public SourceInfoContents Contents { get; private set; }
    public bool Sealed { get; private set; }
    public Type Type { get; private set; }
    public Type[] TypeGenericArguments { get; private set; }
    public string MethodName { get; private set; }
    public MethodInfo Method { get; private set; }
    public Type[] MethodGenericArguments { get; private set; }
    public object SourceInstance { get; private set; }

    public DiagnosticSourceInfo()
    {

    }

    public DiagnosticSourceInfo Seal()
    {
      Sealed = true;
      return this;
    }

    public DiagnosticSourceInfo SetType(Type type, bool deriveGenerics = false)
    {
      if(!Sealed)
      {
        Type = type;
        Contents |= SourceInfoContents.Type;
        if(deriveGenerics)
        {
          SetTypeGenericArguments(type.GenericTypeArguments);
        }
      }
      return this;
    }

    public DiagnosticSourceInfo SetTypeGenericArguments(params Type[] genericArguments)
    {
      if(!Sealed)
      {
        TypeGenericArguments = genericArguments;
        Contents |= SourceInfoContents.TypeGenericArguments;
      }
      return this;
    }

    public DiagnosticSourceInfo SetType(Type type, params Type[] genericArguments)
    {
      SetType(type);
      SetTypeGenericArguments(genericArguments);
      return this;
    }

    public DiagnosticSourceInfo SetMethodName(string name)
    {
      if(!Sealed)
      {
        MethodName = name;
        Contents |= SourceInfoContents.MethodName;
      }
      return this;
    }

    public DiagnosticSourceInfo SetMethodGenericArguments(params Type[] genericArguments)
    {
      if(!Sealed)
      {
        MethodGenericArguments = genericArguments;
        Contents |= SourceInfoContents.MethodGenericArguments;
      }
      return this;
    }

    public DiagnosticSourceInfo SetMethodName(string name, params Type[] genericArguments)
    {
      SetMethodName(name);
      SetMethodGenericArguments(genericArguments);
      return this;
    }

    public DiagnosticSourceInfo SetMethod(MethodInfo method, bool deriveGenerics = false)
    {
      if(!Sealed)
      {
        Method = method;
        Contents |= SourceInfoContents.Method;
        if(deriveGenerics)
        {
          SetMethodName(method.Name, method.GetGenericArguments());
        }
        else
        {
          SetMethodName(method.Name);
        }
      }
      return this;
    }

    public DiagnosticSourceInfo SetMethod(MethodInfo method, params Type[] genericArguments)
    {
      SetMethod(method);
      SetMethodName(method.Name, genericArguments);
      return this;
    }

    public DiagnosticSourceInfo SetSourceInstance(object instance)
    {
      if(!Sealed)
      {
        SourceInstance = instance;
        Contents |= SourceInfoContents.SourceInstance;
      }
      return this;
    }

  }
}
