using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;


namespace WrathTools.Unity
{
  public static class UnityDiagnostics
  {

    public static UnityDiagnosticOptions Options = UnityDiagnosticOptions.Default;

    /// <summary> 
    /// The <see cref="Regex"/> patterns by which to compare the IDs of input <see cref="DiagnosticContext"/>s. Any IDs which match one of these 
    /// patterns will be automatically ignored. Use this if you want to ensure specific <see cref="DiagnosticContext"/>s throw errors.
    /// </summary>
    /// <remarks> Some native WrathTools ID patterns are automatically included. </remarks>
    public readonly static HashSet<Regex> DoNotHandlePatterns = new()
    {
      new Regex(@"^wrath\.serialization\.schema_misaligned.*"),
      new Regex(@"^wrath\.serialization\.missing_schema_field.*")
    };

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

    public static ValueScope<UnityDiagnosticOptions> OptionsScope(UnityDiagnosticOptions options)
    {
      return new ValueScope<UnityDiagnosticOptions>(v => Options = v, Options, options);
    }

  }
}
