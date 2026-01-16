using System;
using System.Diagnostics;


namespace WrathTools
{
  public abstract class DiagnosticContext
  {

    protected Exception _exception;

    public bool PrintStackTrace;
    public virtual string Message { get; protected set; }
    public virtual DiagnosticType DiagnosticType { get; protected set; }
    public virtual bool HasException => _exception != null;
    public virtual StackTrace StackTrace { get; protected set; }

    protected DiagnosticContext(string message, DiagnosticType type, StackTrace stackTrace = null)
    {
      CommonConstruction(message, type, stackTrace);
    }

    protected DiagnosticContext(Exception exception, DiagnosticType type, StackTrace stackTrace = null)
    {
      CommonConstruction(exception.Message, type, stackTrace);
      _exception = exception;
    }

    protected DiagnosticContext(Exception exception, string message, DiagnosticType type, StackTrace stackTrace = null)
    {
      CommonConstruction(message, type, stackTrace);
      _exception = exception;
    }

    public bool TryGetException(out Exception exception)
    {
      exception = _exception;
      return HasException;
    }

    private void CommonConstruction(string message, DiagnosticType type, StackTrace stackTrace)
    {
      Message = message;
      DiagnosticType = type;
      StackTrace = stackTrace;
    }

  }
}
