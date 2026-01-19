using System;


namespace WrathTools
{
  public interface IAutoSaveProvider<TSelf> : ISaveProvider<AutoSaveObject<TSelf>, TSelf>
    where TSelf : class, IAutoSaveProvider<TSelf>
  {

    protected internal Func<TSelf> GetCreator();

  }
}