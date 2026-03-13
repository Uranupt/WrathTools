using System;
using UnityEngine;


namespace WrathTools.Unity
{
  public sealed class UnityDiagnosticHandler : DiagnosticHandler
  {

    private enum SourceType
    {
      Other,
      Unity,
      Native
    }

    public static DiagnosticHandlerFocus GetHandlerFocus() => DiagnosticHandlerFocus.Application;

    public static DiagnosticResponse HandleDiagnostic(DiagnosticContext context, bool isHandled)
    {
      if(!UnityDiagnostics.Active
        || (isHandled && UnityDiagnostics.Options.Has(UnityDiagnosticOptions.IgnoreHandled))
        || UnityDiagnostics.IsIdIgnored(context.ID))
      {
        return DiagnosticResponse.Ignored;
      }

      SourceType sourceType = GetSourceType(context.GetType());
      bool canLog = sourceType switch
      {
        SourceType.Unity => true,
        SourceType.Native => UnityDiagnostics.Options.Has(UnityDiagnosticOptions.LogNativeSources),
        SourceType.Other => UnityDiagnostics.Options.Has(UnityDiagnosticOptions.LogOtherSources),
        _ => false
      };
      bool canHandle = UnityDiagnostics.Options.Has(GetHandleOptions(context.DiagnosticType, sourceType));
      bool canConsume = canHandle && UnityDiagnostics.Options.Has(GetConsumeOptions(context.DiagnosticType));

      if(canLog)
      {
        Log(context.Message, context.DiagnosticType);
      }

      return canConsume ? DiagnosticResponse.Consumed
        : canHandle ? DiagnosticResponse.Handled
        : canLog ? DiagnosticResponse.Accessed
        : DiagnosticResponse.Ignored;

    }

    private static void Log(string message, DiagnosticType type)
    {
      Action<string> log = type switch
      {
        DiagnosticType.Message => Debug.Log,
        DiagnosticType.Warning => Debug.LogWarning,
        DiagnosticType.Error => Debug.LogError,
        _ => null
      };
      log?.Invoke(message);
    }

    private static SourceType GetSourceType(Type type)
    {
      if(type == typeof(UnityDiagnosticContext))
      {
        return SourceType.Unity;
      }
      if(type == typeof(NativeDiagnosticContext))
      {
        return SourceType.Native;
      }
      return SourceType.Other;
    }

    private static UnityDiagnosticOptions GetHandleOptions(DiagnosticType type) => type switch
      {
        DiagnosticType.Message => UnityDiagnosticOptions.HandleMessages,
        DiagnosticType.Warning => UnityDiagnosticOptions.HandleWarnings,
        DiagnosticType.Error => UnityDiagnosticOptions.HandleErrors,
        _ => UnityDiagnosticOptions.None
      };

    private static UnityDiagnosticOptions GetHandleOptions(SourceType source) => source switch
      {
        SourceType.Unity => UnityDiagnosticOptions.None,
        SourceType.Native => UnityDiagnosticOptions.HandleNativeSources,
        SourceType.Other => UnityDiagnosticOptions.HandleOtherSources,
        _ => UnityDiagnosticOptions.None
      };

    private static UnityDiagnosticOptions GetHandleOptions(DiagnosticType type, SourceType source) => GetHandleOptions(type) | GetHandleOptions(source);

    private static UnityDiagnosticOptions GetConsumeOptions(DiagnosticType type) => type switch
      {
        DiagnosticType.Message => UnityDiagnosticOptions.ConsumeHandledMessages,
        DiagnosticType.Warning => UnityDiagnosticOptions.ConsumeHandledWarnings,
        DiagnosticType.Error => UnityDiagnosticOptions.ConsumeHandledErrors,
        _ => UnityDiagnosticOptions.None
      };

  }
}