using System;


namespace WrathTools
{
  internal sealed class HandlerDescriptor
  {

    public readonly DiagnosticHandlerScope Scope;
    public readonly Func<DiagnosticContext, bool, DiagnosticResponse> Handle;

    public HandlerDescriptor(DiagnosticHandlerScope scope, Func<DiagnosticContext, bool, DiagnosticResponse> handle)
    {
      Scope = scope;
      Handle = handle;
    }

  }
}
