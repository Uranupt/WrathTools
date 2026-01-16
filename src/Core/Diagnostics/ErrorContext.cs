using System;


namespace WrathTools
{
  public sealed class ErrorContext : DiagnosticContext
  {

    public ErrorContext(Exception exception) : base(exception, DiagnosticType.Error)
    {
      
    }

    public ErrorContext(Exception exception, string message) : base(exception, message, DiagnosticType.Error)
    {

    }

  }
}
