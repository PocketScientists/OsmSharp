using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Routes
{
  public static class RouteBuilderExtensions
  {
    public delegate Result<Route> BuildRoute(RouterDb routerDb, Profile vehicleProfile, Func<ushort, Factor> getFactor, RouterPoint source, RouterPoint target, List<uint> path);
  }
}
