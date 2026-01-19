

using System;
using System.Diagnostics;

namespace WrathTools.Unity
{
  public static class UnityDiagnostics
  {

    public static UnityDiagnosticOptions Options = UnityDiagnosticOptions.Default;

    public static UnityDiagnosticContext NewMessage(string message, string id = null, StackTrace stackTrace = null,
      DiagnosticSourceInfo sourceInfo = null)
    {
      return UnityDiagnosticContext.NewMessage(message, id, stackTrace, sourceInfo);
    }

    public static UnityDiagnosticContext NewWarning(string message, string id = null, StackTrace stackTrace = null,
      DiagnosticSourceInfo sourceInfo = null)
    {
      return UnityDiagnosticContext.NewWarning(message, id, stackTrace, sourceInfo);
    }

    public static UnityDiagnosticContext NewError(Exception exception, string message = null, string id = null,
      StackTrace stackTrace = null, DiagnosticSourceInfo sourceInfo = null)
    {
      return UnityDiagnosticContext.NewError(exception, message, id, stackTrace, sourceInfo);
    }

    public static void LogMessage(string message, string id = null, StackTrace stackTrace = null,
      DiagnosticSourceInfo sourceInfo = null)
    {
      Diagnostics.Log(UnityDiagnosticContext.NewMessage(message, id, stackTrace, sourceInfo));
    }

    public static void LogWarning(string message, string id = null, StackTrace stackTrace = null,
      DiagnosticSourceInfo sourceInfo = null)
    {
      Diagnostics.Log(UnityDiagnosticContext.NewWarning(message, id, stackTrace, sourceInfo));
    }

    public static void LogError(Exception exception, string message = null, string id = null, StackTrace stackTrace = null,
      DiagnosticSourceInfo sourceInfo = null)
    {
      Diagnostics.Log(UnityDiagnosticContext.NewError(exception, message, id, stackTrace, sourceInfo));
    }

    public static IScope OptionsScope(UnityDiagnosticOptions options)
    {
      return new ValueScope<UnityDiagnosticOptions>(v => Options = v, Options, options);
    }

  }
}
