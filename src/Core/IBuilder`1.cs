

namespace WrathTools
{
  public interface IBuilder<out T> : IBuilder where T : class
  {
    T Build();
  }
}