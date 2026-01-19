using System;


namespace WrathTools
{
  public sealed class ScopeProvider : IScopeProvider
  {

    private readonly Func<IScope> _scopeBuilder;

    public ScopeProvider(Func<IScope> scopeBuilder)
    {
      _scopeBuilder = scopeBuilder;
    }

    public IScope Enter() => _scopeBuilder.Invoke();

  }
}
