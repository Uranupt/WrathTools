using System;
using System.IO;


namespace WrathTools
{
  public interface IBinaryReadable<TSelf> : IBinaryReadable where TSelf : IBinaryReadable<TSelf>
  {

    new Func<BinaryReader, TSelf> GetReader();

  }
}
