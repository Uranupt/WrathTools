using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Xml.Linq;


namespace WrathTools
{
  public abstract class CreatorCollectionBase : ICreatorCollection
  {

    protected virtual Dictionary<ArgsSignature, HashSet<ICreator>> CreatorsByArgs { get; } = new();

    public abstract Type CreatedType { get; }

    public bool HasCreator(params Type[] argTypes) => TryGetCreator(out _, argTypes);
    public bool HasCreator(string name, params Type[] argTypes)
      => TryGetCreator(out _, name, argTypes);
    public bool HasCreator(bool exactArgMatch, params Type[] argTypes) => TryGetCreator(out _, exactArgMatch);
    public bool HasCreator(string name, bool exactArgMatch, params Type[] argTypes)
      => TryGetCreator(out _, name, exactArgMatch, argTypes);

    public bool TryGetCreator(out ICreator creator, params Type[] argTypes)
      => TryGetCreator(out creator, Creators.DefaultCreatorName, false, argTypes);

    public bool TryGetCreator(out ICreator creator, string name, params Type[] argTypes)
      => TryGetCreator(out creator, name, false, argTypes);

    public bool TryGetCreator(out ICreator creator, bool exactArgMatch, params Type[] argTypes)
      => TryGetCreator(out creator, Creators.DefaultCreatorName, exactArgMatch, argTypes);

    public bool TryGetCreator(out ICreator creator, string name, bool exactArgMatch, params Type[] argTypes)
    {
      IEnumerable<ICreator> pool;
      if(exactArgMatch)
      {
        CreatorsByArgs.TryGetValue(new ArgsSignature(argTypes), out HashSet<ICreator> list);
        pool = list;
      }
      else
      {
        pool = CreatorsByArgs.Where(p => p.Key.CanAccept(argTypes)).SelectMany(p => p.Value);
      }
      creator = null;
      if(pool == null){ return false; }
      if(name != Creators.DefaultCreatorName)
      {
        pool = pool.Where(c => c.Name == name);
      }

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
            case (false, true):
            {
              break;
            }
          }
          break;
        }
      }
      return creator != null;
    }

    public ICreator GetCreator(params Type[] argTypes) => GetCreator(Creators.DefaultCreatorName, false, argTypes);
    public ICreator GetCreator(string name, params Type[] argTypes) => GetCreator(name, false, argTypes);
    public ICreator GetCreator(bool exactArgMatch, params Type[] argTypes) => GetCreator(Creators.DefaultCreatorName, exactArgMatch, argTypes);

    public ICreator GetCreator(string name, bool exactArgMatch, params Type[] argTypes)
    {
      if(!TryGetCreator(out ICreator creator, name, exactArgMatch, argTypes))
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

    protected bool AddCreator(ICreator creator)
    {
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