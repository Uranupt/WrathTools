using System;


namespace WrathTools
{
  public interface ICreatorCollection<out TResult> : ICreatorCollection
  {

    new ICreatorFor<TResult> GetCreator(params Type[] argTypes);
    new ICreatorFor<TResult> GetCreator(string name, params Type[] argTypes);
    new ICreatorFor<TResult> GetCreator(bool exactArgLength, bool exactArgTypes, params Type[] argTypes);
    new ICreatorFor<TResult> GetCreator(string name, bool exactArgLength, bool exactArgTypes, params Type[] argTypes);

  }
}