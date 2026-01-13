using System;


namespace WrathTools
{
  /// <summary> Enum representing the state and validity of a <see cref="SaveObject"/></summary>
  [Flags]
  public enum BuildState
  { 
    Incomplete = 0,
    Validated = 1,
    MissingFields = 1 << 1,
    InvalidChildren = 1 << 2
  }

  public static class BuildStateExtensions
  {

    /// <summary> Returns if a <see cref="BuildState"/> value denotes a valid and usable <see cref="SaveObject"/> state. </summary>
    public static bool IsValid(this BuildState state)
    {
      return state == BuildState.Validated;
    }

  }
}