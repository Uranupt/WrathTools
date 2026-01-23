using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;



namespace WrathTools
{
  public static partial class Creators
  {

    private static HashSet<Type> _discoveredConstructors = new();

    private static MethodInfo[] _constructorsByArity = new MethodInfo[]
    {

    };

    private static ICreator NewConstructorN0<TResult>(ConstructorInfo info)
    {

    }

    private static ICreator NewConstructorN1<TArg, TResult>(ConstructorInfo info)
    {

    }

    private static ICreator NewConstructorN2<TArg1, TArg2, TResult>(ConstructorInfo info)
    {

    }

    private static ICreator NewConstructorN3<TArg1, TArg2, TArg3, TResult>(ConstructorInfo info)
    {

    }

    private static ICreator NewConstructorN4<TArg1, TArg2, TArg3, TArg4, TResult>(ConstructorInfo info)
    {

    }

  }
}