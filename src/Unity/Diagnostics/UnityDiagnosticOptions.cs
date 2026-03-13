using System;


namespace WrathTools.Unity
{
  [Flags]
  public enum UnityDiagnosticOptions
  { 
    None = 0,
    IgnoreHandled = 1,
    LogNativeSources = 1 << 1,
    LogOtherSources = 1 << 2,
    HandleNativeSources = 1 << 3,
    HandleOtherSources = 1 << 4,
    HandleMessages = 1 << 5,
    HandleWarnings = 1 << 6,
    HandleErrors = 1 << 7,
    ConsumeHandledMessages = 1 << 8,
    ConsumeHandledWarnings = 1 << 9,
    ConsumeHandledErrors = 1 << 10,
    HandleAllTypes = HandleMessages | HandleWarnings | HandleErrors,
    HandleAllSources = HandleNativeSources | HandleOtherSources,
    HandleAll = HandleAllTypes | HandleAllSources,
    LogAndHandleNative = LogNativeSources | HandleNativeSources,
    LogAndHandleOther = LogOtherSources | HandleOtherSources,
    LogAndHandleAll = LogAndHandleNative | LogAndHandleOther,
    LogAll = LogNativeSources | LogOtherSources,
    ConsumeAllHandledTypes = ConsumeHandledMessages | ConsumeHandledWarnings | ConsumeHandledErrors,
    ConsumeAll = HandleAll | ConsumeAllHandledTypes,
    LogAndConsumeAll = ConsumeAll | LogAll,
    Default = LogNativeSources | HandleNativeSources | HandleAllTypes
  }

  public static class UnityDiagnosticOptionsExtensions
  { 

    public static bool Has(this UnityDiagnosticOptions options, UnityDiagnosticOptions check)
    {
      return (options & check) == check;
    }

    public static bool HasAny(this UnityDiagnosticOptions options, UnityDiagnosticOptions check)
    {
      return (options & check) != 0;
    }

    public static UnityDiagnosticOptions With(this UnityDiagnosticOptions options, UnityDiagnosticOptions with)
    {
      return options | with;
    }

    public static UnityDiagnosticOptions Without(this UnityDiagnosticOptions options, UnityDiagnosticOptions without)
    {
      return options & ~without;
    }

  }
}
