using System;


namespace WrathTools
{
  public interface ICreatorCollection
  {

    Type CreatedType { get; }

    bool HasCreator(params Type[] argTypes);
    bool HasCreator(string name, params Type[] argTypes);
    bool HasCreator(bool exactArgLength, bool exactArgTypes, params Type[] argTypes);
    bool HasCreator(string name, bool exactArgLength, bool exactArgTypes, params Type[] argTypes);
    bool TryGetCreator(out ICreator creator, params Type[] argTypes);
    bool TryGetCreator(out ICreator creator, string name, params Type[] argTypes);
    bool TryGetCreator(out ICreator creator, bool exactArgLength, bool exactArgTypes, params Type[] argTypes);
    bool TryGetCreator(out ICreator creator, string name, bool exactArgLength, bool exactArgTypes, params Type[] argTypes);
    ICreator GetCreator(params Type[] argTypes);
    ICreator GetCreator(string name, params Type[] argTypes);
    ICreator GetCreator(bool exactArgLength, bool exactArgTypes, params Type[] argTypes);
    ICreator GetCreator(string name, bool exactArgLength, bool exactArgTypes, params Type[] argTypes);

  }
}