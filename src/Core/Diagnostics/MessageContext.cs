

namespace WrathTools
{
  public sealed class MessageContext : DiagnosticContext
  {

    public MessageContext(string message) : base(message, DiagnosticType.Message)
    {

    }

  }
}