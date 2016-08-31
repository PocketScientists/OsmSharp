using OsmSharp.Routing.Network;
using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing
{
  public interface IRouter
  {
    bool SupportsAll(params Profile[] profiles);

    Result<RouterPoint> TryResolve(Profile[] profiles, float latitude, float longitude, Func<RoutingEdge, bool> isBetter, float searchDistanceInMeter = 50f);

    Result<bool> TryCheckConnectivity(Profile profile, RouterPoint point, float radiusInMeters);

    Result<Route> TryCalculate(Profile profile, RouterPoint source, RouterPoint target);

    Result<float> TryCalculateWeight(Profile profile, RouterPoint source, RouterPoint target);

    Result<Route[][]> TryCalculate(Profile profile, RouterPoint[] sources, RouterPoint[] targets, ISet<int> invalidSources, ISet<int> invalidTargets);

    Result<float[][]> TryCalculateWeight(Profile profile, RouterPoint[] sources, RouterPoint[] targets, ISet<int> invalidSources, ISet<int> invalidTargets);
  }
}
