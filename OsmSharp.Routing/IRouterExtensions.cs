using OsmSharp.Geo;
using OsmSharp.Routing.Exceptions;
using OsmSharp.Routing.Network;
using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing
{
  public static class IRouterExtensions
  {
    public const float DefaultConnectivityRadius = 250f;

    public static Result<RouterPoint>[] TryResolve(this IRouter router, Profile profile, ICoordinate[] coordinates, float searchDistanceInMeter = 50f)
    {
      if (coordinates == null)
        throw new ArgumentNullException("coordinate");
      Result<RouterPoint>[] resultArray = new Result<RouterPoint>[coordinates.Length];
      for (int index = 0; index < coordinates.Length; ++index)
        resultArray[index] = router.TryResolve(profile, coordinates[index], searchDistanceInMeter);
      return resultArray;
    }

    public static Result<RouterPoint> TryResolve(this IRouter router, Profile profile, ICoordinate coordinate, float searchDistanceInMeter = 50f)
    {
      return router.TryResolve(new Profile[1]{ profile }, coordinate, searchDistanceInMeter);
    }

    public static Result<RouterPoint>[] TryResolve(this IRouter router, Profile[] profiles, ICoordinate[] coordinates, float searchDistanceInMeter = 50f)
    {
      if (coordinates == null)
        throw new ArgumentNullException("coordinate");
      Result<RouterPoint>[] resultArray = new Result<RouterPoint>[coordinates.Length];
      for (int index = 0; index < coordinates.Length; ++index)
        resultArray[index] = router.TryResolve(profiles, coordinates[index], searchDistanceInMeter);
      return resultArray;
    }

    public static Result<RouterPoint> TryResolve(this IRouter router, Profile[] profiles, ICoordinate coordinate, float searchDistanceInMeter = 50f)
    {
      return router.TryResolve(profiles, coordinate.Latitude, coordinate.Longitude, searchDistanceInMeter);
    }

    public static Result<RouterPoint> TryResolve(this IRouter router, Profile profile, float latitude, float longitude, float searchDistanceInMeter = 50f)
    {
      return router.TryResolve(new Profile[1]{ profile }, latitude, longitude, searchDistanceInMeter);
    }

    public static Result<RouterPoint> TryResolve(this IRouter router, Profile[] profiles, float latitude, float longitude, float searchDistanceInMeter = 50f)
    {
      return router.TryResolve(profiles, latitude, longitude, (Func<RoutingEdge, bool>) null, searchDistanceInMeter);
    }

    public static Result<RouterPoint> TryResolve(this IRouter router, Profile[] profiles, ICoordinate coordinate, Func<RoutingEdge, bool> isBetter, float searchDistanceInMeter = 50f)
    {
      return router.TryResolve(profiles, coordinate.Latitude, coordinate.Longitude, isBetter, searchDistanceInMeter);
    }

    public static RouterPoint Resolve(this IRouter router, Profile profile, float latitude, float longitude, float searchDistanceInMeter = 50f)
    {
      return router.Resolve(new Profile[1]{ profile }, latitude, longitude, searchDistanceInMeter);
    }

    public static RouterPoint Resolve(this IRouter router, Profile profile, ICoordinate coordinate, float searchDistanceInMeter = 50f)
    {
      return router.TryResolve(profile, coordinate, searchDistanceInMeter).Value;
    }

    public static RouterPoint Resolve(this IRouter router, Profile[] profiles, ICoordinate coordinate, float searchDistanceInMeter = 50f)
    {
      return router.TryResolve(profiles, coordinate, searchDistanceInMeter).Value;
    }

    public static RouterPoint[] Resolve(this IRouter router, Profile profile, ICoordinate[] coordinates, float searchDistanceInMeter = 50f)
    {
      Result<RouterPoint>[] resultArray = router.TryResolve(profile, coordinates, searchDistanceInMeter);
      RouterPoint[] routerPointArray = new RouterPoint[resultArray.Length];
      for (int index = 0; index < resultArray.Length; ++index)
        routerPointArray[index] = resultArray[index].Value;
      return routerPointArray;
    }

    public static RouterPoint Resolve(this IRouter router, Profile[] profiles, float latitude, float longitude, float searchDistanceInMeter = 50f)
    {
      return router.TryResolve(profiles, latitude, longitude, searchDistanceInMeter).Value;
    }

    public static RouterPoint Resolve(this IRouter router, Profile[] profiles, ICoordinate coordinate, Func<RoutingEdge, bool> isBetter, float searchDistanceInMeter = 50f)
    {
      return router.TryResolve(profiles, coordinate, isBetter, searchDistanceInMeter).Value;
    }

    public static RouterPoint Resolve(this IRouter router, Profile[] profiles, float latitude, float longitude, Func<RoutingEdge, bool> isBetter, float searchDistanceInMeter = 50f)
    {
      return router.TryResolve(profiles, latitude, longitude, isBetter, searchDistanceInMeter).Value;
    }

    public static bool CheckConnectivity(this IRouter router, Profile profile, RouterPoint point, float radiusInMeters)
    {
      return router.TryCheckConnectivity(profile, point, radiusInMeters).Value;
    }

    public static bool CheckConnectivity(this IRouter router, Profile profile, RouterPoint point)
    {
      return router.CheckConnectivity(profile, point, 250f);
    }

    public static Route Calculate(this IRouter router, Profile profile, ICoordinate source, ICoordinate target)
    {
      return router.TryCalculate(profile, source, target).Value;
    }

    public static Route Calculate(this IRouter router, Profile profile, ICoordinate[] locations)
    {
      return router.TryCalculate(profile, locations).Value;
    }

    public static Route Calculate(this IRouter router, Profile profile, RouterPoint[] locations)
    {
      return router.TryCalculate(profile, locations).Value;
    }

    public static Route Calculate(this IRouter router, Profile profile, float sourceLatitude, float sourceLongitude, float targetLatitude, float targetLongitude)
    {
      return router.TryCalculate(profile, sourceLatitude, sourceLongitude, targetLatitude, targetLongitude).Value;
    }

    public static Result<Route> TryCalculate(this IRouter router, Profile profile, ICoordinate source, ICoordinate target)
    {
      return router.TryCalculate(profile, source.Latitude, source.Longitude, target.Latitude, target.Longitude);
    }

    public static Result<Route> TryCalculate(this IRouter router, Profile profile, ICoordinate[] locations)
    {
      if (locations.Length < 2)
        throw new ArgumentOutOfRangeException("Cannot calculate a routing along less than two locations.");
      Result<RouterPoint>[] resultArray = router.TryResolve(profile, locations, 50f);
      Result<Route> result1 = router.TryCalculate(profile, resultArray[0].Value, resultArray[1].Value);
      if (result1.IsError)
        return result1;
      for (int index = 2; index < resultArray.Length; ++index)
      {
        Result<Route> result2 = router.TryCalculate(profile, resultArray[index - 1].Value, resultArray[index].Value);
        if (result2.IsError)
          return result2;
        result1 = new Result<Route>(result1.Value.Concatenate(result2.Value));
      }
      return result1;
    }

    public static Result<Route> TryCalculate(this IRouter router, Profile profile, RouterPoint[] locations)
    {
      if (locations.Length < 2)
        throw new ArgumentOutOfRangeException("Cannot calculate a routing along less than two locations.");
      Result<Route> result1 = router.TryCalculate(profile, locations[0], locations[1]);
      if (result1.IsError)
        return result1;
      for (int index = 2; index < locations.Length; ++index)
      {
        Result<Route> result2 = router.TryCalculate(profile, locations[index - 1], locations[index]);
        if (result2.IsError)
          return result2;
        result1 = new Result<Route>(result1.Value.Concatenate(result2.Value));
      }
      return result1;
    }

    public static Result<Route> TryCalculate(this IRouter router, Profile profile, float sourceLatitude, float sourceLongitude, float targetLatitude, float targetLongitude)
    {
      Profile[] profiles = new Profile[1]{ profile };
      Result<RouterPoint> result1 = router.TryResolve(profiles, sourceLatitude, sourceLongitude, 50f);
      Result<RouterPoint> result2 = router.TryResolve(profiles, targetLatitude, targetLongitude, 50f);
      if (result1.IsError)
        return result1.ConvertError<Route>();
      if (result2.IsError)
        return result2.ConvertError<Route>();
      return router.TryCalculate(profile, result1.Value, result2.Value);
    }

    public static Result<Route[]> TryCalculate(this IRouter router, Profile profile, RouterPoint source, RouterPoint[] targets)
    {
      HashSet<int> intSet1 = new HashSet<int>();
      HashSet<int> intSet2 = new HashSet<int>();
      Result<Route[][]> result1 = router.TryCalculate(profile, new RouterPoint[1]{ source }, targets, (ISet<int>) intSet1, (ISet<int>) intSet2);
      if (intSet1.Count > 0)
        return new Result<Route[]>("Some sources could not be routed from. Most likely there are islands in the loaded network.", (Func<string, Exception>) (s =>
        {
          throw new RouteNotFoundException(s);
        }));
      if (intSet2.Count > 0)
        return new Result<Route[]>("Some targets could not be routed to. Most likely there are islands in the loaded network.", (Func<string, Exception>) (s =>
        {
          throw new RouteNotFoundException(s);
        }));
      if (result1.IsError)
        return result1.ConvertError<Route[]>();
      Route[] result2 = new Route[result1.Value.Length];
      for (int index = 0; index < result1.Value.Length; ++index)
        result2[index] = result1.Value[0][index];
      return new Result<Route[]>(result2);
    }

    public static Route Calculate(this IRouter router, Profile profile, RouterPoint source, RouterPoint target)
    {
      return router.TryCalculate(profile, source, target).Value;
    }

    public static Route[] Calculate(this IRouter router, Profile profile, RouterPoint source, RouterPoint[] targets)
    {
      return router.TryCalculate(profile, source, targets).Value;
    }

    public static Route[][] Calculate(this IRouter router, Profile profile, RouterPoint[] sources, RouterPoint[] targets)
    {
      return router.TryCalculate(profile, sources, targets).Value;
    }

    public static Result<Route[][]> TryCalculate(this IRouter router, Profile profile, RouterPoint[] sources, RouterPoint[] targets)
    {
      HashSet<int> intSet1 = new HashSet<int>();
      HashSet<int> intSet2 = new HashSet<int>();
      Route[][] routeArray = router.TryCalculate(profile, sources, targets, (ISet<int>) intSet1, (ISet<int>) intSet2).Value;
      if (intSet1.Count > 0)
        return new Result<Route[][]>("Some sources could not be routed from. Most likely there are islands in the loaded network.", (Func<string, Exception>) (s =>
        {
          throw new RouteNotFoundException(s);
        }));
      if (intSet2.Count > 0)
        return new Result<Route[][]>("Some targets could not be routed to. Most likely there are islands in the loaded network.", (Func<string, Exception>) (s =>
        {
          throw new RouteNotFoundException(s);
        }));
      Route[][] result = new Route[routeArray.Length][];
      for (int index1 = 0; index1 < routeArray.Length; ++index1)
      {
        result[index1] = new Route[routeArray[index1].Length];
        for (int index2 = 0; index2 < routeArray[index1].Length; ++index2)
          result[index1][index2] = routeArray[index1][index2];
      }
      return new Result<Route[][]>(result);
    }

    public static Route[][] Calculate(this IRouter router, Profile profile, RouterPoint[] sources, RouterPoint[] targets, ISet<int> invalidSources, ISet<int> invalidTargets)
    {
      return router.TryCalculate(profile, sources, targets, invalidSources, invalidTargets).Value;
    }

    public static Result<float> TryCalculateWeight(this IRouter router, Profile profile, ICoordinate source, ICoordinate target)
    {
      return router.TryCalculateWeight(profile, source.Latitude, source.Longitude, target.Latitude, target.Longitude);
    }

    public static Result<float> TryCalculateWeight(this IRouter router, Profile profile, float sourceLatitude, float sourceLongitude, float targetLatitude, float targetLongitude)
    {
      Profile[] profiles = new Profile[1]{ profile };
      Result<RouterPoint> result1 = router.TryResolve(profiles, sourceLatitude, sourceLongitude, 50f);
      Result<RouterPoint> result2 = router.TryResolve(profiles, targetLatitude, targetLongitude, 50f);
      if (result1.IsError)
        return result1.ConvertError<float>();
      if (result2.IsError)
        return result2.ConvertError<float>();
      return router.TryCalculateWeight(profile, result1.Value, result2.Value);
    }

    public static Result<float[][]> TryCalculateWeight(this IRouter router, Profile profile, ICoordinate[] locations)
    {
      return router.TryCalculateWeight(profile, locations, locations);
    }

    public static Result<float[][]> TryCalculateWeight(this IRouter router, Profile profile, ICoordinate[] sources, ICoordinate[] targets)
    {
      RouterPoint[] sources1 = new RouterPoint[sources.Length];
      for (int index = 0; index < sources.Length; ++index)
      {
        Result<RouterPoint> result = router.TryResolve(profile, sources[index], 50f);
        if (result.IsError)
          return new Result<float[][]>(string.Format("Source at index {0} could not be resolved: {1}", new object[2]
          {
            (object) index,
            (object) result.ErrorMessage
          }), (Func<string, Exception>) (s =>
          {
            throw new ResolveFailedException(s);
          }));
        sources1[index] = result.Value;
      }
      RouterPoint[] targets1 = new RouterPoint[targets.Length];
      for (int index = 0; index < targets.Length; ++index)
      {
        Result<RouterPoint> result = router.TryResolve(profile, targets[index], 50f);
        if (result.IsError)
          return new Result<float[][]>(string.Format("Target at index {0} could not be resolved: {1}", new object[2]
          {
            (object) index,
            (object) result.ErrorMessage
          }), (Func<string, Exception>) (s =>
          {
            throw new ResolveFailedException(s);
          }));
        targets1[index] = result.Value;
      }
      HashSet<int> intSet1 = new HashSet<int>();
      HashSet<int> intSet2 = new HashSet<int>();
      Result<float[][]> weight = router.TryCalculateWeight(profile, sources1, targets1, (ISet<int>) intSet1, (ISet<int>) intSet2);
      if (intSet1.Count > 0)
        return new Result<float[][]>("Some sources could not be routed from. Most likely there are islands in the loaded network.", (Func<string, Exception>) (s =>
        {
          throw new RouteNotFoundException(s);
        }));
      if (intSet2.Count > 0)
        return new Result<float[][]>("Some targets could not be routed to. Most likely there are islands in the loaded network.", (Func<string, Exception>) (s =>
        {
          throw new RouteNotFoundException(s);
        }));
      return weight;
    }

    public static Result<float[][]> TryCalculateWeight(this IRouter router, Profile profile, RouterPoint[] locations)
    {
      HashSet<int> intSet = new HashSet<int>();
      Result<float[][]> weight = router.TryCalculateWeight(profile, locations, locations, (ISet<int>) intSet, (ISet<int>) intSet);
      if (intSet.Count > 0)
        return new Result<float[][]>("At least one location could not be routed from/to. Most likely there are islands in the loaded network.", (Func<string, Exception>) (s =>
        {
          throw new RouteNotFoundException(s);
        }));
      return weight;
    }

    public static Result<float[][]> TryCalculateWeight(this IRouter router, Profile profile, RouterPoint[] locations, ISet<int> invalids)
    {
      return router.TryCalculateWeight(profile, locations, locations, invalids, invalids);
    }

    public static float[][] CalculateWeight(this IRouter router, Profile profile, RouterPoint[] locations, ISet<int> invalids)
    {
      return router.TryCalculateWeight(profile, locations, invalids).Value;
    }
  }
}
