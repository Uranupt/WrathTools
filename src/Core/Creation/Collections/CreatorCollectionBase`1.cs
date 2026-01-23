

using System;

namespace WrathTools
{
  public abstract class CreatorCollectionBase<TResult> : CreatorCollectionBase, ICreatorCollection<TResult>
  {

    public override Type CreatedType => typeof(TResult);

    public new ICreatorFor<TResult> GetCreator(params Type[] argTypes)
      => GetCreator(Creators.DefaultCreatorName, false, false, argTypes);

    public new ICreatorFor<TResult> GetCreator(string name, params Type[] argTypes)
      => GetCreator(name, false, false, argTypes);

    public new ICreatorFor<TResult> GetCreator(bool exactArgLength, bool exactArgTypes, params Type[] argTypes)
      => GetCreator(Creators.DefaultCreatorName, exactArgLength, exactArgTypes, argTypes);

    public new ICreatorFor<TResult> GetCreator(string name, bool exactArgLength, bool exactArgTypes, params Type[] argTypes)
      => (ICreatorFor<TResult>)base.GetCreator(name, exactArgLength, exactArgTypes, argTypes);

  }
}