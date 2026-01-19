

namespace WrathTools
{
  public interface IBinaryConvertible<TSelf> : IBinaryConvertible, IBinaryReadable<TSelf> where TSelf : IBinaryConvertible<TSelf>
  {

  }
}