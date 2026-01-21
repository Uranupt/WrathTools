

namespace WrathTools
{ 
  public interface ICreator<out T> : ICreator
  {
    new T Create();
  }
}
