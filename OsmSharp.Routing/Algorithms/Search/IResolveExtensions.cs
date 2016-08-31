using OsmSharp.Routing.Network;
using System;

namespace OsmSharp.Routing.Algorithms.Search
{
  public static class IResolveExtensions
  {
    public delegate IResolver CreateResolver(float latitude, float longitude, Func<RoutingEdge, bool> isBetter);
  }
}
