using System;
using System.IO;


namespace WrathTools
{
  public interface IBinaryReadable
  {

    Func<BinaryReader, IBinaryReadable> GetReader();

  }
}
