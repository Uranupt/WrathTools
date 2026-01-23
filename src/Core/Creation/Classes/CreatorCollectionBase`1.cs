

using System;

namespace WrathTools
{
  public abstract class CreatorCollectionBase<TResult> : CreatorCollectionBase, ICreatorCollection<TResult>
  {

    public override Type CreatedType => typeof(TResult);

    public new ICreatorFor<TResult> GetCreator(params Type[] argTypes)
      => GetCreator(Creators.DefaultCreatorName, false, argTypes);

    public new ICreatorFor<TResult> GetCreator(string name, params Type[] argTypes)
      => GetCreator(name, false, argTypes);

    public new ICreatorFor<TResult> GetCreator(bool exactArgMatch, params Type[] argTypes)
      => GetCreator(Creators.DefaultCreatorName, exactArgMatch, argTypes);

    public new ICreatorFor<TResult> GetCreator(string name, bool exactArgMatch, params Type[] argTypes)
      => (ICreatorFor<TResult>)base.GetCreator(name, exactArgMatch, argTypes);

  }
}