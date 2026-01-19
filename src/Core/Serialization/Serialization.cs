using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;


namespace WrathTools
{
  public static class Serialization
  {

    public static Encoding DefaultEncoding = Encoding.UTF8;

    public static void WriteToStream(IWriteContents source, Stream stream, Encoding encoding = null, int bufferSize = 1024)
    {
      encoding ??= DefaultEncoding;
      using StreamWriter writer = new(stream, encoding, bufferSize, true);
      source.WriteContents(writer);
    }

    public static async Task WriteToStreamAsync(IWriteContentsAsync source, Stream stream, Action onDone = null, Encoding encoding = null, int bufferSize = 1024)
    {
      encoding ??= DefaultEncoding;
      using StreamWriter writer = new(stream, encoding, bufferSize, true);
      await source.WriteContentsAsync(writer);
      onDone?.Invoke();
    }

    public static void WriteToFile(IWriteContents source, string path, Encoding encoding = null, bool append = false, int bufferSize = 1024)
    {
      encoding ??= DefaultEncoding;
      using StreamWriter writer = new(path, append, encoding, bufferSize);
      source.WriteContents(writer);
    }

    public static async Task WriteToFileAsync(IWriteContentsAsync source, string path, Action onDone = null, Encoding encoding = null, bool append = false, int bufferSize = 1024)
    {
      encoding ??= DefaultEncoding;
      using StreamWriter writer = new(path, append, encoding, bufferSize);
      await source.WriteContentsAsync(writer);
      onDone?.Invoke();
    }

  }
}