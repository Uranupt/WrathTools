using System;


namespace WrathTools.Unity
{
  [Flags]
  public enum ItemBehaviors
  {
    None = 0,
    AllowNull = 1,
    SkipDuplicates = 1 << 1,
    ReverseDirection = 1 << 2
  }

  public static class ItemBehaviorsExtensions
  {

    public static bool HasFlag(this ItemBehaviors behaviors, ItemBehaviors check) => (behaviors & check) == check;

  }
}