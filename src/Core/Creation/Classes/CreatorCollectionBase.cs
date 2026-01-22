using System;
using System.Collections.Generic;


namespace WrathTools
{
  public abstract class CreatorCollectionBase : ICreatorCollection
  {

    protected virtual HashSet<ICreator> Creators { get; set; } = new();

    public abstract Type CreatedType { get; }

    public bool HasCreator(params Type[] args) => TryGetCreator(out _, args);

    public bool TryGetCreator(out ICreator creator, params Type[] args)
    {
      foreach(ICreator ctr in Creators)
      {
        if(ArgumentCheck(args, ctr.ArgumentTypes))
        {
          creator = ctr;
          return true;
        }
      }
      creator = null;
      return false;
    }

    public bool TryCreate(out object value, params object[] args)
    {
      foreach(ICreator creator in Creators)
      {
        if(ArgumentCheck(args, creator.ArgumentTypes))
        {
          value = creator.Create(args);
          return true;
        }
      }
      value = default;
      return false;
    }

    public bool TryCreateAs<T>(out T value, params object[] args)
    {
      if(!typeof(T).IsAssignableFrom(CreatedType) && TryCreate(out object resl, args))
      {
        value = (T)resl;
        return true;
      }
      value = default;
      return false;
    }

    public object Create(params object[] args)
    {
      if(!TryCreate(out object value, args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return value;
    }

    public T CreateAs<T>(params object[] args)
    {
      if(typeof(T).IsAssignableFrom(CreatedType))
      {
        Diagnostics.LogError(
          new InvalidOperationException($"The CreatorCollection for Type '{CreatedType.Name}' cannot create an " +
          $"instance assignable to the Type '{typeof(T).Name}'"),
          stackTrace: new(true)
        );
      }
      return (T)Create(args);
    }

    public ICreator GetCreator(params Type[] args)
    {
      if(!TryGetCreator(out ICreator creator, args))
      {
        Diagnostics.LogError(
          new ArgumentException(GetArgumentErrorMessage(args)),
          stackTrace: new(true)
        );
      }
      return creator;
    }

    protected bool ArgumentCheck(Type[] providedArgs, Type[] creatorArgs)
    {
      if(providedArgs.Length != creatorArgs.Length) { return false; }
      for(int i = 0; i < providedArgs.Length; i++)
      {
        if(creatorArgs[i].IsAssignableFrom(providedArgs[i])) { return false; }
      }
      return true;
    }

    protected bool ArgumentCheck(object[] providedArgs, Type[] creatorArgs)
    {
      if(providedArgs.Length != creatorArgs.Length) { return false; }
      for(int i = 0; i < providedArgs.Length; i++)
      {
        if(!creatorArgs[i].IsAssignableFrom(providedArgs[i].GetType())) { return false; }
      }
      return true;
    }

    protected string GetArgumentErrorMessage(params object[] args)
    {
      if(args.Length == 0)
      {
        return $"Failed to find a Creator for the Type '{CreatedType}' with no arguments.";
      }
      string argTypes = $"'{args[0].GetType().Name}'";
      for(int i = 0; i < args.Length; i++)
      {
        argTypes += $", '{args[i].GetType().Name}'";
      }
      return $"Failed to find a Creator for the Type '{CreatedType}' with the argument Types: {argTypes}";
    }

    protected string GetArgumentErrorMessage(params Type[] args)
    {
      if(args.Length == 0)
      {
        return $"Failed to find a Creator for the Type '{CreatedType}' with no arguments.";
      }
      string argTypes = $"'{args[0].Name}'";
      for(int i = 0; i < args.Length; i++)
      {
        argTypes += $", '{args[i].Name}'";
      }
      return $"Failed to find a Creator for the Type '{CreatedType}' with the argument Types: {argTypes}";
    }

  }
}