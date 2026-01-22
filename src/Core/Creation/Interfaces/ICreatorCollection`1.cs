using System;


namespace WrathTools
{
  public interface ICreatorCollection<out TResult> : ICreatorCollection
  {

    new ICreatorFor<TResult> GetCreator(params Type[] args);
    new TResult Create(params object[] args);

  }
}