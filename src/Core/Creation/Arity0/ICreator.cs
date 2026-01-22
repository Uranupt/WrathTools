using System;


namespace WrathTools
{ 
  public interface ICreator
  {

    Type CreatedType { get; }
    Type[] ArgumentTypes { get; }
    object Create(params object[] args);
    bool TryCreateAs(Type type, out object value, params object[] args);
    bool TryCreateAs<T>(out T value, params object[] args);
    object CreateAs(Type type, params object[] args);
    T CreateAs<T>(params object[] args);

  }
}
