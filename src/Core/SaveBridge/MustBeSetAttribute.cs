using System;


namespace WrathTools
{
  /// <summary>
  /// Attribute for use with <see cref="SaveObject"/> fields which should be automatically checked for non-default values
  /// during validation. For fields where the default is valid, use this Attribute on a dedicated bool instead.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class MustBeSetAttribute : Attribute 
  {
    /// <summary> 
    /// The name of an optional boolean field that controls enforcement of validation.
    /// If this property is set, non-default values are only enforced during validation if the named field
    /// is both found and set to true.
    /// </summary>
    public string Condition { get; set; }
  }
}