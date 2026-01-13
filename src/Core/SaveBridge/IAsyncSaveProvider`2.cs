using System;


namespace WrathTools
{
  /// <summary>
  /// Async extension of <see cref="ISaveProvider{T1, T2}"/>, promises async save method <see cref="BuildSaveAsync"/>
  /// </summary>
  public interface IAsyncSaveProvider<TSave, TProvider> : ISaveProvider<TSave, TProvider>
    where TSave : SaveObject<TSave, TProvider>
    where TProvider : class, IAsyncSaveProvider<TSave, TProvider>
  {
    /// <summary> 
    /// Asynchronously attempts to build a <see cref="SaveObject"/> of Type <typeparamref name="TSave"/> and
    /// provides it along with the success status to the given callback.
    /// </summary>
    public void BuildSaveAsync(Action<TSave, bool> onDone);
  }
}