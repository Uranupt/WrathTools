

namespace WrathTools
{
  /// <summary>
  /// Interface for the runtime provider half of the <see cref="SaveObject{T1, T2}"/> and <see cref="ISaveProvider{T1, T2}"/> pair contract.
  /// </summary>
  public interface ISaveProvider<TSave, TProvider> 
    where TSave : SaveObject<TSave, TProvider>
    where TProvider : class, ISaveProvider<TSave, TProvider>
  {
    /// <summary> Builds a new <see cref="SaveObject"/> of Type <typeparamref name="TSave"/>. </summary>
    public TSave BuildSave();
  }
}