using System;
using System.Collections.Generic;
using System.Text;

namespace WrathTools.UnitTests
{

  public sealed class SelfCreator<T>
  {

    private SelfCreator(){}

    [Creator]
    public static SelfCreator<T> Create() => new();

  }

}
