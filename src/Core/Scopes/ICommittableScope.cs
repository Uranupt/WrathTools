

namespace WrathTools
{
  public interface ICommittableScope : IScope
  {

    bool CanCommit { get; }
    bool Committed { get; }
    bool Commit();

  }
}