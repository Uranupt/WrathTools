using System;


namespace WrathTools
{
  public readonly struct ArgsSignature
  {

    private const int HashSeed = 33;
    private const int HashPrime = 47;

    private readonly int _hash;
    public readonly Type[] Types;

    public ArgsSignature(Type[] types)
    {
      Types = types;
      _hash = BuildHash(types);
    }

    public ArgsSignature(object[] args)
    {
      Types = new Type[args.Length];
      for(int i = 0; i < args.Length; i++)
      {
        Types[i] = args[i].GetType();
      }
      _hash = BuildHash(Types);
    }

    private static int BuildHash(Type[] types)
    {
      int resl = HashSeed;
      resl = (resl * HashPrime) + types.Length;
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

    public override int GetHashCode() => _hash;

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