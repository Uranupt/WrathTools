using System;


namespace WrathTools.Deprecated
{
  [Flags]
  [Obsolete("SaveBridge has been deprecated in favor of BinarySerialization")]
  public enum BuildState
  { 
    Incomplete = 0,
    Validated = 1,
    Sealed = Validated,
    MissingFields = 1 << 1,
    InvalidChildren = 1 << 2,
    Consumed = 1 << 3
  }

  [Obsolete("SaveBridge has been deprecated in favor of BinarySerialization")]
  public static class BuildStateExtensions
  {

    public static bool Has(this BuildState state, BuildState check)
    {
      return (state & check) == check;
    }

    public static bool IsValid(this BuildState state)
    {
      return state == BuildState.Validated;
    }

  }
}