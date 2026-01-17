using System.Diagnostics;


namespace WrathTools
{
  public sealed class UnityWarningContext : DiagnosticContext
  {

    public static void Log(string message, StackTrace stackTrace = null)
    {
      Diagnostics.Log(new UnityWarningContext(message, stackTrace));
    }

    public UnityWarningContext(string message, StackTrace stackTrace = null) : base(message, DiagnosticType.Warning, stackTrace)
    {

    }

  }
}
