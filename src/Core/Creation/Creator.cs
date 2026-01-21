using System;


namespace WrathTools
{
  public sealed class Creator : ICreator
  {

    private Func<object> _create;
    public Type CreatedType { get; private set; }

    public Creator(Type createdType, Func<object> create)
    {
      CreatedType = createdType;
      _create = create;
    }

    public object Create() => _create?.Invoke();

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

    public bool TryCreateAs<T>(out T value)
    {
      if(TryCreateAs(typeof(T), out object obj))
      {
        value = (T)obj;
        return false;
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

    public T CreateAs<T>() => (T)CreateAs(typeof(T));

  }
}