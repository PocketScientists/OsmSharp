using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms
{
  public interface IWeightMatrixAlgorithm : IAlgorithm
  {
    Dictionary<int, LocationError> Errors { get; }

    List<RouterPoint> RouterPoints { get; }

    float[][] Weights { get; }

    int IndexOf(int locationIdx);

    int LocationIndexOf(int routerPointIdx);
  }
}
