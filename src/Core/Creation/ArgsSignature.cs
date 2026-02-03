using System;


namespace WrathTools
{
  public readonly struct ArgsSignature
  {

    private const int HashSeed = 33;
    private const int HashPrime = 47;
    private const int EmptyHash = HashSeed * HashPrime;

    private readonly int _hash;
    private readonly Type[] _types;

    private int Hash => _types != null ? _hash : EmptyHash;
    public Type[] Types => _types ?? Array.Empty<Type>();

    public ArgsSignature(Type[] types)
    {
      _types = types;
      _hash = BuildHash(types);
    }

    public ArgsSignature(object[] args)
    {
      _types = new Type[args.Length];
      for(int i = 0; i < args.Length; i++)
      {
        _types[i] = args[i].GetType();
      }
      _hash = BuildHash(_types);
    }

    public static string GetTypesString(params Type[] types)
    {
      if(types.Length == 0) { return "None"; }
      string resl = types[0].Name;
      for(int i = 1; i < types.Length; i++)
      {
        resl += $", {types[i].Name}";
      }
      return resl;
    }

    public static string GetTypesString(params object[] values)
    {
      if(values.Length == 0) { return "None"; }
      string resl = values[0].GetType().Name;
      for(int i = 1; i < values.Length; i++)
      {
        resl += $", {values[i].GetType().Name}";
      }
      return resl;
    }

    private static int BuildHash(Type[] types)
    {
      int resl = EmptyHash + types.Length;
      for(int i = 0; i < types.Length; i++)
      {
        resl = (resl * HashPrime) + types[i].GetHashCode();
      }
      return resl;
    }

    public static bool operator ==(ArgsSignature sig, ArgsSignature other) => sig.Equals(other);
    public static bool operator !=(ArgsSignature sig, ArgsSignature other) => !sig.Equals(other);

    public override bool Equals(object other)
    {
      return other is ArgsSignature oSig && oSig.GetHashCode() == GetHashCode();
    }

    public override int GetHashCode() => Hash;

    public bool CanAccept(params Type[] types)
    {
      if(types.Length < Types.Length) { return false; }
      for(int i = 0; i < Types.Length; i++)
      {
        if(!Types[i].IsAssignableFrom(types[i])) {  return false; }
      }
      return true;
    }

    public bool CanAccept(params object[] args)
    {
      if(args.Length < Types.Length) { return false; }
      for(int i = 0; i < Types.Length; i++)
      {
        if(!Types[i].IsAssignableFrom(args[i].GetType())) { return false; }
      }
      return true;
    }

    public bool ArgsTypeMatch(bool allowOverflow, params Type[] argTypes)
    {
      if(argTypes.Length < Types.Length 
        || (!allowOverflow && argTypes.Length != Types.Length)) 
      { 
        return false; 
      }
      for(int i = 0; i < Types.Length; i++)
      {
        if(Types[i] != argTypes[i]) { return false; }
      }
      return true;
    }

    public bool ArgsTypeMatch(bool allowOverflow, params object[] args)
    {
      if(args.Length < Types.Length
        || (!allowOverflow && args.Length != Types.Length))
      {
        return false;
      }
      for(int i = 0; i < Types.Length; i++)
      {
        if(Types[i] != args[i].GetType()) { return false; }
      }
      return true;
    }


  }
}