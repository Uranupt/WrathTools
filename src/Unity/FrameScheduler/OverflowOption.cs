

namespace WrathTools.Unity
{
  /// <summary>
  /// Option for Enumerable Jobs if the Enumerator jump count exceeds available items.
  /// </summary>
  public enum OverflowOption
  {
    EndWork,
    Clamp,
    Wrap,
    UseDefault
  }
}