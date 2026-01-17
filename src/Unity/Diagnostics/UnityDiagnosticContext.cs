using System.Diagnostics;
using System;


namespace WrathTools.Unity
{
  public abstract class UnityDiagnosticContext : DiagnosticContext
  {

    protected UnityDiagnosticContext(string message, DiagnosticType diagnosticType, StackTrace stackTrace = null)
      :base(message, diagnosticType, stackTrace)
    {

    }

    protected UnityDiagnosticContext(Exception exception, DiagnosticType diagnosticType, string message = null, StackTrace stackTrace = null)
      :base(exception, diagnosticType, message, stackTrace)
    {

    }

  }
}
