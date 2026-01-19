using System;
using System.Diagnostics;


namespace WrathTools
{
  public abstract class DiagnosticContext
  {

    private static string _undefinedID = "UNDEF";

    /// <summary> The ID used when one is not supplied. </summary>
    public static string UndefinedID
    {
      get => _undefinedID;
      set
      {
        _undefinedID = value ?? "UNDEF";
      }
    }

    public DiagnosticType DiagnosticType { get; private set; }

    public virtual string Message { get; protected set; }
    public virtual Exception Exception { get; protected set;  }
    public virtual string ID { get; protected set; }
    public virtual DiagnosticSourceInfo SourceInfo { get; protected set; }

    protected void InitializeError(Exception exception, string message = null, string id = null, StackTrace stackTrace = null, 
      DiagnosticSourceInfo sourceInfo = null)
    {
      CommonConstruction(message ?? exception.ToString(), DiagnosticType.Error, id, stackTrace, sourceInfo);
      Exception = exception;
    }

    protected void InitializeWarning(string message, string id = null, StackTrace stackTrace = null, DiagnosticSourceInfo sourceInfo = null)
    {
      CommonConstruction(message, DiagnosticType.Warning, id, stackTrace, sourceInfo);
    }

    protected void InitializeMessage(string message, string id = null, StackTrace stackTrace = null, DiagnosticSourceInfo sourceInfo = null)
    {
      CommonConstruction(message, DiagnosticType.Message, id, stackTrace, sourceInfo);
    }

    protected void AppendStackTrace(StackTrace stackTrace)
    {
      Message = Message + "\n" + Diagnostics.WriteStackTrace(stackTrace);
    }

    private void CommonConstruction(string message, DiagnosticType type, string id, StackTrace stackTrace, DiagnosticSourceInfo sourceInfo)
    {
      Message = message;
      DiagnosticType = type;
      ID = id ?? UndefinedID;
      SourceInfo = sourceInfo;
      if(stackTrace != null)
      {
        AppendStackTrace(stackTrace);
      }
    }

  }
}
