using System.Reflection;
using System;
using System.Threading.Tasks;


namespace WrathTools
{
  internal class BinarySerializerMethodSelector
  {

    public readonly bool HasSync;
    public readonly bool HasAsync;
    public readonly MethodInfo Write;
    public readonly MethodInfo Read;
    public readonly MethodInfo WriteAsync;
    public readonly MethodInfo ReadAsync;

    public BinarySerializerMethodSelector(Type declaringType, Type targetedType)
    {
      Write = null;
      Read = null;
      WriteAsync = null;
      ReadAsync = null;

      foreach(MethodInfo method in declaringType.GetMethods())
      {
        if(method.Name == "Write")
        {
          ParameterInfo[] parameters = method.GetParameters();
          if(parameters.Length == 2
            && parameters[0].ParameterType == typeof(BinaryWriteContext)
            && parameters[1].ParameterType == targetedType)
          {
            Write = method;
          }
        }
        else if(method.Name == "Read")
        {
          ParameterInfo[] parameters = method.GetParameters();
          if(parameters.Length == 1
            && parameters[0].ParameterType == typeof(BinaryReadContext)
            && method.ReturnType == targetedType)
          {
            Read = method;
          }
        }
        else if(method.Name == "WriteAsync")
        {
          ParameterInfo[] parameters = method.GetParameters();
          if(parameters.Length == 2
            && parameters[0].ParameterType == typeof(BinaryWriteContext)
            && parameters[1].ParameterType == targetedType
            && method.ReturnType == typeof(Task))
          {
            WriteAsync = method;
          }
        }
        else if(method.Name == "ReadAsync")
        {
          ParameterInfo[] parameters = method.GetParameters();
          if(parameters.Length == 1
            && parameters[0].ParameterType == typeof(BinaryReadContext)
            && method.ReturnType == typeof(Task<>).MakeGenericType(targetedType))
          {
            ReadAsync = method;
          }
        }
      }

      if(Write == null || Read == null)
      {
        Write = null;
        Read = null;
      }
      if(WriteAsync == null || ReadAsync == null)
      {
        WriteAsync = null;
        ReadAsync = null;
      }
      HasSync = Write != null && Read != null;
      HasAsync = WriteAsync != null && ReadAsync != null;
    }

  }
}