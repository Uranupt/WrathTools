using System.IO;
using System.Threading.Tasks;


namespace WrathTools
{
  public interface IWriteContentsAsync : IWriteContents
  {
    Task WriteContentsAsync(StreamWriter writer);
  }
}
