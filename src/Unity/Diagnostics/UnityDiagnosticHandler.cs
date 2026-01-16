using System;
using UnityEngine;


namespace WrathTools.Unity
{
  public sealed class UnityDiagnosticHandler : DiagnosticHandler
  {

    private enum ContextType
    {
      Other,
      Unity,
      Native
    }

    public static bool Active = true;
    public static bool ThrowErrors = false;
    public static bool IgnoreHandled = false;
    public static bool LogNative = true;
    public static bool MarkNativeHandled = true;
    public static bool LogOther = false;
    public static bool MarkOtherHandled = false;

    public static DiagnosticHandlerScope GetScope() => DiagnosticHandlerScope.Application;

    public static DiagnosticResponse Handle(DiagnosticContext context, bool isHandled)
    {
      if(!Active || (isHandled && IgnoreHandled)) { return DiagnosticResponse.Ignored; }
      ContextType contextType = GetContextType(context.GetType());
      if(contextType == ContextType.Other && !LogOther) { return DiagnosticResponse.Ignored; }
      if(contextType == ContextType.Native && !LogNative) {  return DiagnosticResponse.Ignored; }
      switch(context.DiagnosticType)
      {
        case DiagnosticType.Message:
        {
          Debug.Log(context.Message);
          break;
        }
        case DiagnosticType.Warning:
        {
          Debug.LogWarning(context.Message);
          break;
        }
        case DiagnosticType.Error:
        {
          if(!context.TryGetException(out Exception e))
          {
            e = new Exception(context.Message);
          }
          Debug.LogException(e);
          if(ThrowErrors) { return DiagnosticResponse.Accessed; }
          break;
        }
      }
      return contextType switch
      {
        ContextType.Unity => DiagnosticResponse.Handled,
        ContextType.Native => MarkNativeHandled ? DiagnosticResponse.Handled : DiagnosticResponse.Accessed,
        ContextType.Other => MarkOtherHandled ? DiagnosticResponse.Handled : DiagnosticResponse.Accessed
      };
    }

    private static ContextType GetContextType(Type type)
    {
      if(type == typeof(UnityErrorContext) || type == typeof(UnityWarningContext) || type == typeof(UnityMessageContext))
      {
        return ContextType.Unity;
      }
      if(type == typeof(ErrorContext) || type == typeof(WarningContext) || type == typeof(MessageContext))
      {
        return ContextType.Native;
      }
      return ContextType.Other;
    }

  }
}