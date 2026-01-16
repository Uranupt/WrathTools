

namespace WrathTools
{
  public sealed class WarningContext : DiagnosticContext
  {

    public WarningContext(string message) : base(message, DiagnosticType.Warning)
    {

    }
     
  }
}
