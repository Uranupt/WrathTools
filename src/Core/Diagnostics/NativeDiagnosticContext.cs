using System.Diagnostics;
using System;


namespace WrathTools
{
  public sealed class NativeDiagnosticContext : DiagnosticContext
  {

    public static NativeDiagnosticContext NewMessage(string message, string id = null, StackTrace stackTrace = null,
      DiagnosticSourceInfo sourceInfo = null)
    {
      NativeDiagnosticContext context = new();
      context.InitializeMessage(message, id, stackTrace, sourceInfo);
      return context;
    }

    public static NativeDiagnosticContext NewWarning(string message, string id = null, StackTrace stackTrace = null,
      DiagnosticSourceInfo sourceInfo = null)
    {
      NativeDiagnosticContext context = new();
      context.InitializeWarning(message, id, stackTrace, sourceInfo);
      return context;
    }

    public static NativeDiagnosticContext NewError(Exception exception, string message = null, string id = null, 
      StackTrace stackTrace = null, DiagnosticSourceInfo sourceInfo = null)
    {
      NativeDiagnosticContext context = new();
      context.InitializeError(exception, message, id, stackTrace, sourceInfo);
      return context;
    }

    public static void LogMessage(string message, string id = null, StackTrace stackTrace = null,
      DiagnosticSourceInfo sourceInfo = null)
    {
      Diagnostics.Log(NewMessage(message, id, stackTrace, sourceInfo));
    }

    public static void LogWarning(string message, string id = null, StackTrace stackTrace = null,
      DiagnosticSourceInfo sourceInfo = null)
    {
      Diagnostics.Log(NewWarning(message, id, stackTrace, sourceInfo));
    }

    public static void LogError(Exception exception, string message = null, string id = null, StackTrace stackTrace = null,
      DiagnosticSourceInfo sourceInfo = null)
    {
      Diagnostics.Log(NewError(exception, message, id, stackTrace, sourceInfo));
    }

    private NativeDiagnosticContext()
    {

    }

  }
}
