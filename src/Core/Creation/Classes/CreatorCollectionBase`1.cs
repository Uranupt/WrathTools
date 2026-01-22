

using System;

namespace WrathTools
{
  public abstract class CreatorCollectionBase<TResult> : CreatorCollectionBase, ICreatorCollection<TResult>
  {

    public new ICreatorFor<TResult> GetCreator(params Type[] args) => (ICreatorFor<TResult>)base.GetCreator(args);
    public new TResult Create(params object[] args) => CreateAs<TResult>(args);

  }
}