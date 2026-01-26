

namespace WrathTools
{
  public enum SerializationBehavior
  {
    /// <summary> Static Read and Write methods will be provided. Nothing will be automatically serialized. </summary>
    Manual,
    /// <summary> Only fields marked by SerializeBinary will be serialized. </summary>
    MarkedFields,
    /// <summary> Only public fields and those marked by SerializeBinary will be serialized. </summary>
    PublicFields,
    /// <summary> All fields will be serialized. </summary>
    AllFields
  }
}