using System;

namespace OsmSharp.Routing.Algorithms
{
  public abstract class AlgorithmBase : IAlgorithm
  {
    public bool HasRun { get; protected set; }

    public bool HasSucceeded { get; protected set; }

    public string ErrorMessage { get; protected set; }

    protected void CheckHasRun()
    {
      if (!this.HasRun)
        throw new Exception("No results available, Algorithm has not run yet!");
    }

    protected void CheckHasRunAndHasSucceeded()
    {
      this.CheckHasRun();
      if (!this.HasSucceeded)
        throw new Exception("No results available, Algorithm was not successful!");
    }

    public void Run()
    {
      if (this.HasRun)
        throw new Exception("Algorithm has run already, use a new instance for each run. Use HasRun to check.");
      this.DoRun();
      this.HasRun = true;
    }

    protected abstract void DoRun();
  }
}
