using System;


namespace WrathTools.Unity
{
  [Flags]
  public enum UnityDiagnosticOptions
  { 
    None = 0,
    Active = 1,
    IgnoreHandled = 1 << 1,
    LogNativeSources = 1 << 2,
    LogOtherSources = 1 << 3,
    HandleNativeSources = 1 << 4,
    HandleOtherSources = 1 << 5,
    HandleMessages = 1 << 6,
    HandleWarnings = 1 << 7,
    HandleErrors = 1 << 8,
    ConsumeHandledMessages = 1 << 9,
    ConsumeHandledWarnings = 1 << 10,
    ConsumeHandledErrors = 1 << 11,
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
    Default = Active | LogNativeSources | HandleNativeSources | HandleAllTypes,
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
