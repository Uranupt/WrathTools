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

    public static void Log(DiagnosticContext context)
    {
      Initialize();
      bool handled = false;
      for(int i = 0; i < _handlers.Count; i++)
      {
        DiagnosticResponse response = _handlers[i].Handle.Invoke(context, handled);
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

    public static T ThrowOrDefault<T>(DiagnosticContext context)
    {
      Log(context);
      return default;
    }

    public static bool Try(Action action, Func<Exception, DiagnosticContext> contextBuilder = null, 
      Action<Exception> onCatch = null, Action onFinally = null)
    {
      contextBuilder ??= e => new ErrorContext(e);
      bool resl;
      try
      {
        action.Invoke();
        resl = true;
      }
      catch(Exception e)
      {
        onCatch?.Invoke(e);
        Log(contextBuilder.Invoke(e));
        resl = false;
      }
      finally
      {
        onFinally?.Invoke();
      }
      return resl;
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
        MethodInfo getScope = handler.GetMethods(BindingFlags.Static | BindingFlags.Public)
          .Where(m => m.Name == "GetScope" && m.ReturnType == typeof(DiagnosticHandlerScope))
          .FirstOrDefault();
        MethodInfo handleMethod = handler.GetMethods(BindingFlags.Static | BindingFlags.Public)
          .Where(m => m.Name == "Handle" && m.ReturnType == typeof(DiagnosticResponse) && HandleParameterCheck(m.GetParameters()))
          .FirstOrDefault();
        if(getScope == null || handleMethod == null) { continue; }
        DiagnosticHandlerScope scope = (DiagnosticHandlerScope)getScope.Invoke(null, null);
        Func<DiagnosticContext, bool, DiagnosticResponse> handle = (Func<DiagnosticContext, bool, DiagnosticResponse>)Delegate.CreateDelegate(
          typeof(Func<DiagnosticContext, bool, DiagnosticResponse>), 
          handleMethod
        );
        _handlers.Add(new HandlerDescriptor(scope, handle));
      }

      _handlers.Sort((x, y) => (int)x.Scope - (int)y.Scope);
    }

  }
}
