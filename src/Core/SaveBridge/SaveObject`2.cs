using System;
using System.IO;


namespace WrathTools
{
  /// <summary>
  /// Base class for the SaveObject half of the <see cref="SaveObject{T1, T2}"/> and <see cref="ISaveProvider{T1, T2}"/> pair contract.
  /// </summary>
  public abstract class SaveObject<TSave, TProvider> : SaveObject
    where TSave : SaveObject<TSave, TProvider>
    where TProvider : class, ISaveProvider<TSave, TProvider>
  {

    public override Type Type => typeof(TProvider);

    /// <summary>
    /// Attempts to construct a new instance of Type <typeparamref name="TProvider"/> using the <see cref="SaveObject"/>'s data.
    /// Automatically calls <see cref="SaveObject.MarkConsumed"/>. Provides null and returns false if unsuccessful.
    /// </summary>
    public bool TryLoad(out TProvider instance) => TryLoad<TProvider>(out instance);

    /// <summary>
    /// Attempts to asynchronously construct a new instance of Type <typeparamref name="TProvider"/> using the <see cref="SaveObject"/>'s data,
    /// and passes it, along with the success status, to the provided callback. Automatically calls <see cref="SaveObject.MarkConsumed"/>.
    /// </summary>
    public void LoadAsync(Action<TProvider, bool> onDone) => LoadAsync<TProvider>(onDone);

    /// <summary>
    /// Builds and returns an instance of Type <typeparamref name="TProvider"/>. 
    /// </summary>
    protected abstract TProvider LoadProtected();
    /// <summary>
    /// Asynchronously builds an instance of Type <typeparamref name="TProvider"/> and provides it along with success status to the given callback. 
    /// </summary>
    protected abstract void LoadAsyncProtected(Action<TProvider, bool> onDone);

    protected internal sealed override void LoadInternal<T>(out T instance)
    {
      if(typeof(T) != this.Type)
      {
        throw new InvalidOperationException("Improper Type provided to LoadInternal: " + typeof(T).Name 
          + " Expected: " + typeof(TProvider).Name);
      }
      instance = LoadProtected() as T;
    }

    protected internal sealed override void LoadAsyncInternal<T>(Action<T, bool> onDone)
    {
      if(typeof(T) != this.Type)
      {
        throw new InvalidOperationException("Improper Type provided to LoadAsyncInternal: " + typeof(T).Name
          + " Expected: " + typeof(TProvider).Name);
      }
      LoadAsyncProtected(onDone as Action<TProvider, bool>);
    }

  }
}