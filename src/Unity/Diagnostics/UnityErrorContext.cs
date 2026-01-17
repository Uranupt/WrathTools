using System;
using System.Diagnostics;


namespace WrathTools.Unity
{
  public sealed class UnityErrorContext : UnityDiagnosticContext
  {

    public static void Log(Exception exception, string message = null, StackTrace stackTrace = null)
    {
      Diagnostics.Log(new UnityErrorContext(exception, message, stackTrace));
    }

    public static T ThrowOrDefault<T>(Exception exception, string message = null, StackTrace stackTrace = null)
    {
      return Diagnostics.ThrowOrDefault<T>(new UnityErrorContext(exception, message, stackTrace));
    }

    public UnityErrorContext(Exception exception, string message = null, StackTrace stackTrace = null) 
      : base(exception, DiagnosticType.Error, message, stackTrace)
    {

    }

  }
}
