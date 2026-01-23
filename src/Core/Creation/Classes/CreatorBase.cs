using System;


namespace WrathTools
{
  public abstract class CreatorBase : ICreator
  {

    private string _argumentNames;

    public abstract Type CreatedType { get; }
    public abstract ArgsSignature Signature { get; }
    public abstract string Name { get; }

    public string ArgumentNames
    {
      get
      {
        if(_argumentNames == null)
        {
          if(Signature.Types.Length == 0)
          {
            _argumentNames = "None";
          }
          else
          {
            _argumentNames = $"'{Signature.Types[0].Name}'";
            for(int i = 1; i < Signature.Types.Length; i++)
            {
              _argumentNames += $", {Signature.Types[i].Name}";
            }
          }
        }
        return _argumentNames;
      }
    }

    public abstract bool TryCreate(out object value, params object[] args);

    public bool TryCreateAs<T>(out T value, params object[] args)
    {
      if(typeof(T).IsAssignableFrom(CreatedType) && TryCreateAs(out object resl, args))
      {
        value = (T)resl;
        return true;
      }
      value = default;
      return false;
    }

    public object Create(params object[] args)
    {
      if(!TryCreate(out object resl, args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return resl;
    }

    public T CreateAs<T>(params object[] args)
    {
      if(!typeof(T).IsAssignableFrom(CreatedType))
      {
        Diagnostics.LogError(
          new InvalidOperationException($"The Creator for Type '{CreatedType.Name}' cannot create an " +
          $"instance assignable to the Type '{typeof(T).Name}'"),
          stackTrace: new(true)
        );
      }
      return (T)Create(args);
    }

    protected string GetArgumentErrorMessage(params object[] args)
    {
      if(args.Length > 0)
      {
        string argTypes = $"'{args[0].GetType().Name}'";
        for(int i = 1; i < args.Length; i++)
        {
          argTypes += $", '{args[i].GetType().Name}'";
        }
        return $"The Creator failed to create a new instance of Type '{CreatedType.Name}' from the provided " +
          $"argument Types: {argTypes}. Expected: {ArgumentNames}";
      }
      else
      {
        return $"The Creator failed to create a new instance of Type {CreatedType.Name} without provided arguments." +
          $" Expected: {ArgumentNames}";
      }
    }

  }
}
