using System;


namespace WrathTools
{
  public interface ICreatorCollection<out TResult> : ICreatorCollection
  {

    new ICreatorFor<TResult> GetCreator(params Type[] argTypes);
    new ICreatorFor<TResult> GetCreator(string name, params Type[] argTypes);
    new ICreatorFor<TResult> GetCreator(bool exactArgMatch, params Type[] argTypes);
    new ICreatorFor<TResult> GetCreator(string name, bool exactArgMatch, params Type[] argTypes);

  }
}