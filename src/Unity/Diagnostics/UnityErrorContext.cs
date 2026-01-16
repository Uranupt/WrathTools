using System;

namespace WrathTools.Unity
{
  public sealed class UnityErrorContext : DiagnosticContext
  {

    public UnityErrorContext(Exception exception) : base(exception, DiagnosticType.Error)
    {

    }

    public UnityErrorContext(Exception exception, string message) : base(exception, message, DiagnosticType.Error)
    {

    }

  }
}
