using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;



namespace WrathTools
{
  public static partial class Creators
  {

    private readonly static MethodInfo _collectionCreator = typeof(Creators).GetMethod("NewCollection", BindingFlags.NonPublic | BindingFlags.Static);

    private readonly static MethodInfo[] _creatorsByArity = new MethodInfo[]
    {
      typeof(Creators).GetMethod("NewCreatorN0", BindingFlags.NonPublic | BindingFlags.Static),
      typeof(Creators).GetMethod("NewCreatorN1", BindingFlags.NonPublic | BindingFlags.Static),
      typeof(Creators).GetMethod("NewCreatorN2", BindingFlags.NonPublic | BindingFlags.Static),
      typeof(Creators).GetMethod("NewCreatorN3", BindingFlags.NonPublic | BindingFlags.Static),
      typeof(Creators).GetMethod("NewCreatorN4", BindingFlags.NonPublic | BindingFlags.Static),
      typeof(Creators).GetMethod("NewCreatorN5", BindingFlags.NonPublic | BindingFlags.Static),
      typeof(Creators).GetMethod("NewCreatorN6", BindingFlags.NonPublic | BindingFlags.Static),
      typeof(Creators).GetMethod("NewCreatorN7", BindingFlags.NonPublic | BindingFlags.Static),
      typeof(Creators).GetMethod("NewCreatorN8", BindingFlags.NonPublic | BindingFlags.Static),
      typeof(Creators).GetMethod("NewCreatorN9", BindingFlags.NonPublic | BindingFlags.Static),
      typeof(Creators).GetMethod("NewCreatorN10", BindingFlags.NonPublic | BindingFlags.Static),
      typeof(Creators).GetMethod("NewCreatorN11", BindingFlags.NonPublic | BindingFlags.Static),
      typeof(Creators).GetMethod("NewCreatorN12", BindingFlags.NonPublic | BindingFlags.Static),
      typeof(Creators).GetMethod("NewCreatorN13", BindingFlags.NonPublic | BindingFlags.Static),
      typeof(Creators).GetMethod("NewCreatorN14", BindingFlags.NonPublic | BindingFlags.Static),
      typeof(Creators).GetMethod("NewCreatorN15", BindingFlags.NonPublic | BindingFlags.Static),
    };

    private static CreatorCollectionBase NewCollection<TResult>() => new CreatorCollection<TResult>();


    private static ICreator NewCreatorN0<TResult>(
      Expression expression, ParameterExpression[] _, string name)
    {
      return new Creator<TResult>(
        Expression.Lambda<Func<TResult>>(
          expression)
        .Compile(),
        name
      );
    }

    private static ICreator NewCreatorN1<T, TResult>(
      Expression expression, ParameterExpression[] lambdaParams, string name)
    {
      return new Creator<T, TResult>(
        Expression.Lambda<Func<T, TResult>>(
          expression, lambdaParams)
        .Compile(),
        name
      );
    }

    private static ICreator NewCreatorN2<T1, T2, TResult>(
      Expression expression, ParameterExpression[] lambdaParams, string name)
    {
      return new Creator<T1,T2, TResult>(
        Expression.Lambda<Func<T1, T2, TResult>>(
          expression, lambdaParams)
        .Compile(),
        name
      );
    }

    private static ICreator NewCreatorN3<T1, T2, T3, TResult>(
      Expression expression, ParameterExpression[] lambdaParams, string name)
    {
      return new Creator<T1, T2, T3, TResult>(
        Expression.Lambda<Func<T1, T2, T3, TResult>>(
          expression, lambdaParams)
        .Compile(),
        name
      );
    }

    private static ICreator NewCreatorN4<T1, T2, T3, T4, TResult>(
      Expression expression, ParameterExpression[] lambdaParams, string name)
    {
      return new Creator<T1, T2, T3, T4, TResult>(
        Expression.Lambda<Func<T1, T2, T3, T4, TResult>>(
          expression, lambdaParams)
        .Compile(),
        name
      );
    }

    private static ICreator NewCreatorN5<T1, T2, T3, T4, T5, TResult>(
      Expression expression, ParameterExpression[] lambdaParams, string name)
    {
      return new Creator<T1, T2, T3, T4, T5, TResult>(
        Expression.Lambda<Func<T1, T2, T3, T4, T5, TResult>>(
          expression, lambdaParams)
        .Compile(),
        name
      );
    }

    private static ICreator NewCreatorN6<T1, T2, T3, T4, T5, T6, TResult>(
      Expression expression, ParameterExpression[] lambdaParams, string name)
    {
      return new Creator<T1, T2, T3, T4, T5, T6, TResult>(
        Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, TResult>>(
          expression, lambdaParams)
        .Compile(),
        name
      );
    }

    private static ICreator NewCreatorN7<T1, T2, T3, T4, T5, T6, T7, TResult>(
      Expression expression, ParameterExpression[] lambdaParams, string name)
    {
      return new Creator<T1, T2, T3, T4, T5, T6, T7, TResult>(
        Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, TResult>>(
          expression, lambdaParams)
        .Compile(),
        name
      );
    }

    private static ICreator NewCreatorN8<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
      Expression expression, ParameterExpression[] lambdaParams, string name)
    {
      return new Creator<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>(
          expression, lambdaParams)
        .Compile(),
        name
      );
    }

    private static ICreator NewCreatorN9<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
      Expression expression, ParameterExpression[] lambdaParams, string name)
    {
      return new Creator<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
        Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>>(
          expression, lambdaParams)
        .Compile(),
        name
      );
    }

    private static ICreator NewCreatorN10<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
      Expression expression, ParameterExpression[] lambdaParams, string name)
    {
      return new Creator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(
        Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>>(
          expression, lambdaParams)
        .Compile(),
        name
      );
    }

    private static ICreator NewCreatorN11<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
      Expression expression, ParameterExpression[] lambdaParams, string name)
    {
      return new Creator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(
        Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>>(
          expression, lambdaParams)
        .Compile(),
        name
      );
    }

    private static ICreator NewCreatorN12<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
      Expression expression, ParameterExpression[] lambdaParams, string name)
    {
      return new Creator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
        Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>>(
          expression, lambdaParams)
        .Compile(),
        name
      );
    }

    private static ICreator NewCreatorN13<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
      Expression expression, ParameterExpression[] lambdaParams, string name)
    {
      return new Creator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(
        Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>>(
          expression, lambdaParams)
        .Compile(),
        name
      );
    }

    private static ICreator NewCreatorN14<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
      Expression expression, ParameterExpression[] lambdaParams, string name)
    {
      return new Creator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(
        Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>>(
          expression, lambdaParams)
        .Compile(),
        name
      );
    }

    private static ICreator NewCreatorN15<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
      Expression expression, ParameterExpression[] lambdaParams, string name)
    {
      return new Creator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(
        Expression.Lambda<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>>(
          expression, lambdaParams)
        .Compile(),
        name
      );
    }

  }
}