using System;


namespace WrathTools
{
  [Flags]
  public enum BuildState
  { 
    Incomplete = 0,
    Validated = 1,
    Sealed = Validated,
    MissingFields = 1 << 1,
    InvalidChildren = 1 << 2,
    Consumed = 1 << 3
  }

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