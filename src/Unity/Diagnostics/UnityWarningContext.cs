

namespace WrathTools
{
  public sealed class UnityWarningContext : DiagnosticContext
  {

    public UnityWarningContext(string message) : base(message, DiagnosticType.Warning)
    {

    }

  }
}
