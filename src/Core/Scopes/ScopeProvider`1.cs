using System;


namespace WrathTools
{
  public sealed class ScopeProvider<T> : IScopeProvider<T> where T : IScope
  {

    private readonly Func<T> _scopeBuilder;

    public ScopeProvider(Func<T> scopeBuilder)
    {
      _scopeBuilder = scopeBuilder;
    }

    public T Enter() => _scopeBuilder.Invoke();
    IScope IScopeProvider.Enter() => Enter();

  }
}