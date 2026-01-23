using System;


namespace WrathTools
{ 
  public interface ICreator
  {

    Type CreatedType { get; }
    ArgsSignature Signature { get; }
    public string Name { get; }
    bool TryCreate(out object value, params object[] args);
    bool TryCreateAs<T>(out T value, params object[] args);
    object Create(params object[] args);
    T CreateAs<T>(params object[] args);

  }
}
