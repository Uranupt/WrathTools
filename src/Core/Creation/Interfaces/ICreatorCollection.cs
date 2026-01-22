using System;


namespace WrathTools
{
  public interface ICreatorCollection
  {

    Type CreatedType { get; }

    bool HasCreator(params Type[] args);
    bool TryGetCreator(out ICreator creator, params Type[] args);
    bool TryCreate(out object value, params object[] args);
    bool TryCreateAs<T>(out T value, params object[] args);
    ICreator GetCreator(params Type[] args);
    object Create(params object[] args);
    T CreateAs<T>(params object[] args);

  }
}