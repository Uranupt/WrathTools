using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Xml.Linq;


namespace WrathTools
{
  public abstract class CreatorCollectionBase : ICreatorCollection, ICreatorCollectionInternal
  {

    protected virtual Dictionary<ArgsSignature, HashSet<ICreator>> CreatorsByArgs { get; } = new();

    public abstract Type CreatedType { get; }

    public bool HasCreator(params Type[] argTypes) => TryGetCreator(out _, argTypes);

    public bool HasCreator(string name, params Type[] argTypes)
      => TryGetCreator(out _, name, argTypes);

    public bool HasCreator(bool exactArgCount, bool exactArgTypes, params Type[] argTypes) 
      => TryGetCreator(out _, exactArgCount, exactArgTypes, argTypes);

    public bool HasCreator(string name, bool exactArgCount, bool exactArgTypes, params Type[] argTypes)
      => TryGetCreator(out _, name, exactArgCount, exactArgTypes, argTypes);

    public bool TryGetCreator(out ICreator creator, params Type[] argTypes)
      => TryGetCreator(out creator, Creators.DefaultCreatorName, false, false, argTypes);

    public bool TryGetCreator(out ICreator creator, string name, params Type[] argTypes)
      => TryGetCreator(out creator, name, false, false, argTypes);

    public bool TryGetCreator(out ICreator creator, bool exactArgLength, bool exactArgTypes, params Type[] argTypes)
      => TryGetCreator(out creator, Creators.DefaultCreatorName, exactArgLength, exactArgTypes, argTypes);

    public bool TryGetCreator(out ICreator creator, string name, bool exactArgLength, bool exactArgTypes, params Type[] argTypes)
    {
      IEnumerable<ICreator> pool;
      if(exactArgLength && exactArgTypes)
      {
        if(!CreatorsByArgs.TryGetValue(new ArgsSignature(argTypes), out HashSet<ICreator> set))
        {
          creator = null;
          return false;
        }
        pool = set;
      }
      else
      {
        pool = CreatorsByArgs.Where(p => p.Key.CanAccept(argTypes))
          .SelectMany(p => p.Value)
          .Where(c =>
            (!exactArgLength || c.Signature.Types.Length == argTypes.Length)
            && (!exactArgTypes || c.Signature.ArgsTypeMatch(true, argTypes))
            && (name != Creators.DefaultCreatorName || c.Name == name)
          );
      }
      creator = null;
      foreach(ICreator ctr in pool)
      {
        if(creator == null)
        {
          creator = ctr;
          continue;
        }
        if(creator.Signature.Types.Length > ctr.Signature.Types.Length) { continue; }
        if((creator.Name != name && ctr.Name == name)
          || creator.Signature.Types.Length < ctr.Signature.Types.Length)
        {
          creator = ctr;
          continue;
        }
        for(int i = 0; i < ctr.Signature.Types.Length; i++)
        {
          switch(
            (ctr.Signature.Types[i] == argTypes[i], creator.Signature.Types[i] == argTypes[i])
          )
          {
            case (false, false):
            case (true, true):
            {
              if(i == ctr.Signature.Types.Length - 1)
              {
                //If two have no name preference, no length preference, and no Type match preference, choice is ambiguous.
                return false; 
              }
              continue;
            }
            case (true, false):
            {
              creator = ctr;
              break;
            }
          }
          break;
        }
      }
      return creator != null;
    }

    public ICreator GetCreator(params Type[] argTypes) => GetCreator(Creators.DefaultCreatorName, false, false, argTypes);

    public ICreator GetCreator(string name, params Type[] argTypes) 
      => GetCreator(name, false, false, argTypes);

    public ICreator GetCreator(bool exactArgLength, bool exactArgTypes, params Type[] argTypes) 
      => GetCreator(Creators.DefaultCreatorName, exactArgLength, exactArgTypes, argTypes);

    public ICreator GetCreator(string name, bool exactArgLength, bool exactArgTypes, params Type[] argTypes)
    {
      if(!TryGetCreator(out ICreator creator, name, exactArgLength, exactArgTypes, argTypes))
      {
        string msg = name != Creators.DefaultCreatorName
          ? $"Failed to find a Creator for the Type '{CreatedType.Name}' named '{name}' with the argument Types: {TypesToString(argTypes)}"
          : $"Failed to find a Creator for the Type '{CreatedType.Name}' with the argument Types: {TypesToString(argTypes)}";
        Diagnostics.LogError(
          new Exception(msg),
          stackTrace: new(true)
        );
      }
      return creator;
    }

    protected virtual bool AddCreator(ICreator creator)
    {
      if(creator.CreatedType != CreatedType) { return false; }
      if(!CreatorsByArgs.TryGetValue(creator.Signature, out HashSet<ICreator> set))
      {
        set = new HashSet<ICreator>();
        CreatorsByArgs[creator.Signature] = set;
      }
      foreach(ICreator other in set)
      {
        if(other != creator && other.Name == creator.Name) { return false; }
      }
      set.Add(creator);
      return true;
    }

    bool ICreatorCollectionInternal.AddCreator(ICreator creator) => AddCreator(creator);

    protected string TypesToString(params Type[] args)
    {
      if(args.Length == 0) { return "None"; }
      string argTypes = $"'{args[0].Name}'";
      for(int i = 1; i < args.Length; i++)
      {
        argTypes += $", '{args[i].Name}'";
      }
      return argTypes;
    }

  }
}