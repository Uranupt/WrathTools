

namespace WrathTools
{
  public interface IStronglyCommittableScope : ICommittableScope
  {

    new void Commit();

  }
}
