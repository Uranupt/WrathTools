using System;


namespace WrathTools
{
  [Flags]
  public enum SourceInfoContents
  {
    None = 0,
    Type = 1,
    MethodName = 1 << 1,
    Method = 1 << 2,
    SourceInstance = 1 << 3,
    TypeGenericArguments = 1 << 4,
    MethodGenericArguments = 1 << 5
  }

  public static class SourceInfoContentsExtensions
  {

    public static bool Has(this SourceInfoContents contents, SourceInfoContents check)
    {
      return (contents & check) == check;
    }

    public static bool HasAny(this SourceInfoContents contents, SourceInfoContents check)
    {
      return (contents & check) != 0;
    }

  }
   
}
