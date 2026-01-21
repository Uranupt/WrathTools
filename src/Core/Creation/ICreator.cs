using System;


namespace WrathTools
{ 
  public interface ICreator
  {

    Type CreatedType { get; }
    object Create();
    bool TryCreateAs(Type type, out object value);
    bool TryCreateAs<T>(out T value);
    object CreateAs(Type type);
    T CreateAs<T>();

  }
}
