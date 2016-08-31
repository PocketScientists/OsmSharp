namespace OsmSharp.Routing.Algorithms
{
  public interface IAlgorithm
  {
    bool HasRun { get; }

    bool HasSucceeded { get; }

    string ErrorMessage { get; }

    void Run();
  }
}
