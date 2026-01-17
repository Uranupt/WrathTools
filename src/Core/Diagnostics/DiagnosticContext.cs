using System;
using System.Diagnostics;


namespace WrathTools
{
  public abstract class DiagnosticContext
  {
    public virtual string Message { get; protected set; }
    public virtual DiagnosticType DiagnosticType { get; protected set; }
    public virtual Exception Exception { get; protected set;  }
    protected DiagnosticContext(string message, DiagnosticType type, StackTrace stackTrace = null)
    {
      CommonConstruction(message, type, stackTrace);
    }

    protected DiagnosticContext(Exception exception, DiagnosticType type, string message = null, StackTrace stackTrace = null)
    {
      CommonConstruction(message ?? exception.Message, type, stackTrace);
      Exception = exception;
    }

    protected void AppendStackTrace(StackTrace stackTrace)
    {
      Message = Message + "\n" + Diagnostics.WriteStackTrace(stackTrace);
    }

    private void CommonConstruction(string message, DiagnosticType type, StackTrace stackTrace)
    {
      Message = message;
      DiagnosticType = type;
      if(stackTrace != null)
      {
        AppendStackTrace(stackTrace);
      }
    }

  }
}
