namespace OsmSharp.Routing.Algorithms.Search
{
  public interface IResolver : IAlgorithm
  {
    RouterPoint Result { get; }
  }
}
