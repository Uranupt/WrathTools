using System;


namespace WrathTools
{
  public sealed class Creator<T> : ICreator<T>
  {

    private readonly Func<T> _create;
    public Type CreatedType => typeof(T);

    public Creator(Func<T> create)
    {
      _create = create;
    }

    public T Create() => _create.Invoke();

    object ICreator.Create() => Create();

    public bool TryCreateAs(Type type, out object value)
    {
      if(type.IsAssignableFrom(CreatedType))
      {
        value = Create();
        return true;
      }
      value = default;
      return false;
    }

    public bool TryCreateAs<TOut>(out TOut value)
    {
      if(TryCreateAs(typeof(TOut), out object resl))
      {
        value = (TOut)resl;
        return true;
      }
      value = default;
      return false;
    }

    public object CreateAs(Type type)
    {
      if(!TryCreateAs(type, out object resl))
      {
        Diagnostics.LogError(
          new InvalidOperationException($"The Creator could not create an instance assignable to the Type '{type.Name}'. " +
            $"Creator Type: '{CreatedType.Name}'"),
          stackTrace: new(true)
        );
      }
      return resl;
    }

    public TOut CreateAs<TOut>() => (TOut)CreateAs(typeof(TOut));

  }
}