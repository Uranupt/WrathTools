

namespace WrathTools
{ 
  public sealed class CreatorCollection<TResult> : CreatorCollectionBase<TResult>
  {

    public CreatorCollection()
    {

    }

    public new bool AddCreator(ICreator creator) => base.AddCreator(creator);

  }
}