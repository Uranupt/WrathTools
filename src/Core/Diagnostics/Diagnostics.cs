using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.Diagnostics;
using System.Text;


namespace WrathTools
{
  public static class Diagnostics
  {

    private static bool _initialized = false;
    private static List<HandlerDescriptor> _handlers = new();

    public static NativeDiagnosticContext NewMessage(string message, string id = null, StackTrace stackTrace = null,
          DiagnosticSourceInfo sourceInfo = null)
    {
      return NativeDiagnosticContext.NewMessage(message, id, stackTrace, sourceInfo);
    }

    public static NativeDiagnosticContext NewWarning(string message, string id = null, StackTrace stackTrace = null,
      DiagnosticSourceInfo sourceInfo = null)
    {
      return NativeDiagnosticContext.NewWarning(message, id, stackTrace, sourceInfo);
    }

    public static NativeDiagnosticContext NewError(Exception exception, string message = null, string id = null,
      StackTrace stackTrace = null, DiagnosticSourceInfo sourceInfo = null)
    {
      return NativeDiagnosticContext.NewError(exception, message, id, stackTrace, sourceInfo);
    }

    public static void Log(DiagnosticContext context)
    {
      Initialize();
      bool handled = false;
      for(int i = 0; i < _handlers.Count; i++)
      {
        DiagnosticResponse response = _handlers[i].HandleDiagnostic.Invoke(context, handled);
        if(response == DiagnosticResponse.Consumed) { return; }
        if(response == DiagnosticResponse.Handled)
        {
          handled = true;
        }
      }
      if(handled) { return; }
      if(context.DiagnosticType == DiagnosticType.Error)
      {
        Exception e = context.Exception ?? new Exception(context.Message);
        throw e;
      }
      else
      {
        System.Diagnostics.Debug.Write(context.Message);
      }
    }

    public static void LogMessage(string message, string id = null, StackTrace stackTrace = null,
      DiagnosticSourceInfo sourceInfo = null)
    {
      Log(NativeDiagnosticContext.NewMessage(message, id, stackTrace, sourceInfo));
    }

    public static void LogWarning(string message, string id = null, StackTrace stackTrace = null,
      DiagnosticSourceInfo sourceInfo = null)
    {
      Log(NativeDiagnosticContext.NewWarning(message, id, stackTrace, sourceInfo));
    }

    public static void LogError(Exception exception, string message = null, string id = null, StackTrace stackTrace = null,
      DiagnosticSourceInfo sourceInfo = null)
    {
      Log(NativeDiagnosticContext.NewError(exception, message, id, stackTrace, sourceInfo));
    }

    public static string WriteStackTrace(StackTrace stackTrace)
    {
      StringBuilder builder = new();
      foreach(StackFrame frame in stackTrace.GetFrames())
      {
        MethodBase method = frame.GetMethod();
        builder.Append("at ")
          .Append(method.DeclaringType.FullName)
          .Append(".")
          .Append(method.Name)
          .Append(" in ")
          .Append(frame.GetFileName())
          .Append(": line ")
          .Append(frame.GetFileLineNumber())
          .AppendLine();
      }
      return builder.ToString();
    }

    private static void Initialize()
    {
      if(_initialized) { return; }
      _initialized = true;
      Type[] handlers = AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a => a.GetTypes())
        .Where(t => t.IsSealed && t.IsSubclassOf(typeof(DiagnosticHandler)))
        .ToArray();

      static bool HandleParameterCheck(ParameterInfo[] parameters)
      {
        return parameters.Length == 2
          && parameters[0].ParameterType == typeof(DiagnosticContext)
          && parameters[1].ParameterType == typeof(bool);
      }

      foreach(Type handler in handlers)
      {
        MethodInfo getFocus = handler.GetMethods(BindingFlags.Static | BindingFlags.Public)
          .Where(m => m.Name == "GetHandlerFocus" && m.ReturnType == typeof(DiagnosticHandlerFocus))
          .FirstOrDefault();
        MethodInfo handleMethod = handler.GetMethods(BindingFlags.Static | BindingFlags.Public)
          .Where(m => m.Name == "HandleDiagnostic" && m.ReturnType == typeof(DiagnosticResponse) && HandleParameterCheck(m.GetParameters()))
          .FirstOrDefault();
        if(getFocus == null || handleMethod == null) { continue; }
        DiagnosticHandlerFocus focus = (DiagnosticHandlerFocus)getFocus.Invoke(null, null);
        Func<DiagnosticContext, bool, DiagnosticResponse> handleDiagnostic = (Func<DiagnosticContext, bool, DiagnosticResponse>)Delegate.CreateDelegate(
          typeof(Func<DiagnosticContext, bool, DiagnosticResponse>), 
          handleMethod
        );
        _handlers.Add(new HandlerDescriptor(focus, handleDiagnostic));
      }

      _handlers.Sort((x, y) => (int)x.Focus - (int)y.Focus);
    }

  }
}
