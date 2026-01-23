using System;


namespace WrathTools
{
  public interface ICreatorCollection
  {

    Type CreatedType { get; }

    bool HasCreator(params Type[] argTypes);
    bool HasCreator(string name, params Type[] argTypes);
    bool HasCreator(bool exactArgMatch, params Type[] argTypes);
    bool HasCreator(string name, bool exactArgMatch, params Type[] argTypes);
    bool TryGetCreator(out ICreator creator, params Type[] argTypes);
    bool TryGetCreator(out ICreator creator, string name, params Type[] argTypes);
    bool TryGetCreator(out ICreator creator, bool exactArgMatch, params Type[] argTypes);
    bool TryGetCreator(out ICreator creator, string name, bool exactArgMatch, params Type[] argTypes);
    ICreator GetCreator(params Type[] argTypes);
    ICreator GetCreator(string name, params Type[] argTypes);
    ICreator GetCreator(bool exactArgMatch, params Type[] argTypes);
    ICreator GetCreator(string name, bool exactArgMatch, params Type[] argTypes);

  }
}