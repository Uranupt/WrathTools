using System.Diagnostics;


namespace WrathTools.Unity
{
  public sealed class UnityMessageContext : UnityDiagnosticContext
  {

    public static void Log(string message, StackTrace stackTrace = null)
    {
      Diagnostics.Log(new UnityMessageContext(message, stackTrace));
    }

    public UnityMessageContext(string message, StackTrace stackTrace = null) : base(message, DiagnosticType.Message, stackTrace)
    {

    }

  }
}
