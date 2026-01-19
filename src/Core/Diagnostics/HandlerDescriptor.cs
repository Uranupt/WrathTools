using System;


namespace WrathTools
{
  internal sealed class HandlerDescriptor
  {

    public readonly DiagnosticHandlerFocus Focus;
    public readonly Func<DiagnosticContext, bool, DiagnosticResponse> HandleDiagnostic;

    public HandlerDescriptor(DiagnosticHandlerFocus focus, Func<DiagnosticContext, bool, DiagnosticResponse> handleDiagnostic)
    {
      Focus = focus;
      HandleDiagnostic = handleDiagnostic;
    }

  }
}
