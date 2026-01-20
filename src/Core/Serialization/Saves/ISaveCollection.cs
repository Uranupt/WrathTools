using System.Collections.Generic;


namespace WrathTools
{
  public interface ISaveCollection
  { 

    IReadOnlyCollection<SaveObject> Saves { get; }

  }
}
