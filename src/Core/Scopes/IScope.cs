using System;


namespace WrathTools
{
  public interface IScope : IDisposable
  {

    bool Active { get; }
    void Exit();

  }
}