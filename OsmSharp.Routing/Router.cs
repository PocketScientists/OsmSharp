using OsmSharp.Collections.Tags;
using OsmSharp.Logging;
using OsmSharp.Routing.Algorithms;
using OsmSharp.Routing.Algorithms.Contracted;
using OsmSharp.Routing.Algorithms.Default;
using OsmSharp.Routing.Algorithms.Routes;
using OsmSharp.Routing.Algorithms.Search;
using OsmSharp.Routing.Data;
using OsmSharp.Routing.Exceptions;
using OsmSharp.Routing.Graphs.Directed;
using OsmSharp.Routing.Graphs.Geometric;
using OsmSharp.Routing.Network;
using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing
{
  public class Router : IRouter
  {
    private readonly RouterDb _db;

    public ProfileFactorCache ProfileFactorCache { get; set; }

    public IResolveExtensions.CreateResolver CreateCustomResolver { get; set; }

    public RouteBuilderExtensions.BuildRoute CustomRouteBuilder { get; set; }

    public bool VerifyAllStoppable { get; set; }

    public RouterDb Db
    {
      get
      {
        return this._db;
      }
    }

    public Router(RouterDb db)
    {
      this._db = db;
      this.VerifyAllStoppable = false;
    }

    public bool SupportsAll(params Profile[] profiles)
    {
      return this._db.SupportsAll(profiles);
    }

    public Result<RouterPoint> TryResolve(Profile[] profiles, float latitude, float longitude, Func<RoutingEdge, bool> isBetter, float maxSearchDistance = 50f)
    {
      if (!this._db.SupportsAll(profiles))
        return new Result<RouterPoint>("Not all routing profiles are supported.", (Func<string, Exception>) (message => (Exception) new ResolveFailedException(message)));
      IResolver resolver;
      if (this.CreateCustomResolver == null)
      {
        Func<GeometricEdge, bool> isBetter1 = (Func<GeometricEdge, bool>) null;
        if (isBetter != null)
          isBetter1 = (Func<GeometricEdge, bool>) (edge => isBetter(this._db.Network.GetEdge(edge.Id)));
        Func<GeometricEdge, bool> isAcceptable = this.GetIsAcceptable(profiles);
        resolver = (IResolver) new ResolveAlgorithm(this._db.Network.GeometricGraph, latitude, longitude, this._db.Network.MaxEdgeDistance / 2f, maxSearchDistance, isAcceptable, isBetter1);
      }
      else
        resolver = this.CreateCustomResolver(latitude, longitude, isBetter);
      resolver.Run();
      if (!resolver.HasSucceeded)
        return new Result<RouterPoint>(resolver.ErrorMessage, (Func<string, Exception>) (message => (Exception) new ResolveFailedException(message)));
      return new Result<RouterPoint>(resolver.Result);
    }

    public Result<bool> TryCheckConnectivity(Profile profile, RouterPoint point, float radiusInMeters)
    {
      if (!this._db.Supports(profile))
        return new Result<bool>("Routing profile is not supported.", (Func<string, Exception>) (message => new Exception(message)));
      Func<ushort, Factor> getFactor = this.GetGetFactor(profile);
      OsmSharp.Routing.Algorithms.Default.Dykstra dykstra = new OsmSharp.Routing.Algorithms.Default.Dykstra(this._db.Network.GeometricGraph.Graph, getFactor, (IEnumerable<Path>) point.ToPaths(this._db, getFactor, true), radiusInMeters, false);
      dykstra.Run();
      if (!dykstra.HasSucceeded)
        return new Result<bool>(false);
      return new Result<bool>(dykstra.MaxReached);
    }

    public Result<Route> TryCalculate(Profile profile, RouterPoint source, RouterPoint target)
    {
      if (!this._db.Supports(profile))
        return new Result<Route>("Routing profile is not supported.", (Func<string, Exception>) (message => new Exception(message)));
      Func<ushort, Factor> getFactor = this.GetGetFactor(profile);
      DirectedMetaGraph contracted;
      float weight;
      List<uint> path;
      if (this._db.TryGetContracted(profile, out contracted))
      {
        OsmSharp.Routing.Algorithms.Contracted.BidirectionalDykstra bidirectionalDykstra = new OsmSharp.Routing.Algorithms.Contracted.BidirectionalDykstra(contracted, (IEnumerable<Path>) source.ToPaths(this._db, getFactor, true), (IEnumerable<Path>) target.ToPaths(this._db, getFactor, false));
        bidirectionalDykstra.Run();
        if (!bidirectionalDykstra.HasSucceeded)
          return new Result<Route>(bidirectionalDykstra.ErrorMessage, (Func<string, Exception>) (message => (Exception) new RouteNotFoundException(message)));
        path = bidirectionalDykstra.GetPath(out weight);
      }
      else
      {
        OsmSharp.Routing.Algorithms.Default.BidirectionalDykstra bidirectionalDykstra = new OsmSharp.Routing.Algorithms.Default.BidirectionalDykstra(new OsmSharp.Routing.Algorithms.Default.Dykstra(this._db.Network.GeometricGraph.Graph, getFactor, (IEnumerable<Path>) source.ToPaths(this._db, getFactor, true), float.MaxValue, false), new OsmSharp.Routing.Algorithms.Default.Dykstra(this._db.Network.GeometricGraph.Graph, getFactor, (IEnumerable<Path>) target.ToPaths(this._db, profile, false), float.MaxValue, true));
        bidirectionalDykstra.Run();
        if (!bidirectionalDykstra.HasSucceeded)
          return new Result<Route>(bidirectionalDykstra.ErrorMessage, (Func<string, Exception>) (message => (Exception) new RouteNotFoundException(message)));
        path = bidirectionalDykstra.GetPath(out weight);
      }
      if ((int) source.EdgeId == (int) target.EdgeId)
      {
        Path from = source.PathTo(this._db, profile, target);
        if (from != null && (double) from.Weight < (double) weight)
        {
          path = new List<uint>(2);
          for (; from != null; from = from.From)
            path.Insert(0, from.Vertex);
        }
      }
      return this.BuildRoute(profile, source, target, path);
    }

    public Result<float> TryCalculateWeight(Profile profile, RouterPoint source, RouterPoint target)
    {
      if (!this._db.Supports(profile))
        return new Result<float>("Routing profile is not supported.", (Func<string, Exception>) (message => new Exception(message)));
      throw new NotImplementedException();
    }

    public Result<Route[][]> TryCalculate(Profile profile, RouterPoint[] sources, RouterPoint[] targets, ISet<int> invalidSources, ISet<int> invalidTargets)
    {
      if (!this._db.Supports(profile))
        return new Result<Route[][]>("Routing profile is not supported.", (Func<string, Exception>) (message => new Exception(message)));
      Func<ushort, Factor> getFactor = this.GetGetFactor(profile);
      DirectedMetaGraph contracted;
      if (this._db.TryGetContracted(profile, out contracted))
        Log.TraceEvent("Router", TraceEventType.Warning, "Many to many route calculations are not possible yet using contracted algorithms.");
      ManyToMany manyToMany = new ManyToMany(this._db, getFactor, sources, targets, float.MaxValue);
      manyToMany.Run();
      if (!manyToMany.HasSucceeded)
        return new Result<Route[][]>(manyToMany.ErrorMessage, (Func<string, Exception>) (message => (Exception) new RouteNotFoundException(message)));
      Route[][] result1 = new Route[sources.Length][];
      List<uint> uintList = new List<uint>();
      for (int source = 0; source < sources.Length; ++source)
      {
        result1[source] = new Route[targets.Length];
        for (int target = 0; target < targets.Length; ++target)
        {
          Path path = manyToMany.GetPath(source, target);
          if (path != null)
          {
            uintList.Clear();
            path.AddToList(uintList);
            Result<Route> result2 = this.BuildRoute(profile, sources[source], targets[target], uintList);
            if (result2.IsError)
              return result2.ConvertError<Route[][]>();
            result1[source][target] = result2.Value;
          }
        }
      }
      return new Result<Route[][]>(result1);
    }

    public Result<float[][]> TryCalculateWeight(Profile profile, RouterPoint[] sources, RouterPoint[] targets, ISet<int> invalidSources, ISet<int> invalidTargets)
    {
      if (!this._db.Supports(profile))
        return new Result<float[][]>("Routing profile is not supported.", (Func<string, Exception>) (message => new Exception(message)));
      Func<ushort, Factor> getFactor = this.GetGetFactor(profile);
      DirectedMetaGraph contracted;
      float[][] weights;
      if (this._db.TryGetContracted(profile, out contracted))
      {
        ManyToManyBidirectionalDykstra bidirectionalDykstra = new ManyToManyBidirectionalDykstra(this._db, profile, sources, targets);
        bidirectionalDykstra.Run();
        if (!bidirectionalDykstra.HasSucceeded)
          return new Result<float[][]>(bidirectionalDykstra.ErrorMessage, (Func<string, Exception>) (message => (Exception) new RouteNotFoundException(message)));
        weights = bidirectionalDykstra.Weights;
      }
      else
      {
        ManyToMany manyToMany = new ManyToMany(this._db, getFactor, sources, targets, float.MaxValue);
        manyToMany.Run();
        if (!manyToMany.HasSucceeded)
          return new Result<float[][]>(manyToMany.ErrorMessage, (Func<string, Exception>) (message => (Exception) new RouteNotFoundException(message)));
        weights = manyToMany.Weights;
      }
      int[] numArray = new int[targets.Length];
      for (int index1 = 0; index1 < weights.Length; ++index1)
      {
        int num = 0;
        for (int index2 = 0; index2 < weights[index1].Length; ++index2)
        {
          if (index2 != index1 && (double) weights[index1][index2] == 3.40282346638529E+38)
          {
            ++num;
            ++numArray[index2];
            if (numArray[index2] > (sources.Length - 1) / 2)
              invalidTargets.Add(index2);
          }
        }
        if (num > (targets.Length - 1) / 2)
          invalidSources.Add(index1);
      }
      return new Result<float[][]>(weights);
    }

    protected Func<GeometricEdge, bool> GetIsAcceptable(Profile[] profiles)
    {
      if (this.ProfileFactorCache != null && this.ProfileFactorCache.ContainsAll(profiles))
        return this.ProfileFactorCache.GetIsAcceptable(this.VerifyAllStoppable, profiles);
      return (Func<GeometricEdge, bool>) (edge =>
      {
        float distance;
        ushort profile;
        EdgeDataSerializer.Deserialize(edge.Data[0], out distance, out profile);
        TagsCollectionBase attributes = this._db.EdgeProfiles.Get((uint) profile);
        for (int index = 0; index < profiles.Length; ++index)
        {
          if ((double) profiles[index].Factor(attributes).Value <= 0.0 || this.VerifyAllStoppable && !profiles[index].CanStopOn(attributes))
            return false;
        }
        return true;
      });
    }

    protected Func<ushort, Factor> GetGetFactor(Profile profile)
    {
      if (this.ProfileFactorCache != null)
      {
        if (this.ProfileFactorCache.ContainsAll(profile))
          return this.ProfileFactorCache.GetGetFactor(profile);
      }
      return (Func<ushort, Factor>) (p => profile.Factor(this.Db.EdgeProfiles.Get((uint) p)));
    }

    protected Result<Route> BuildRoute(Profile profile, RouterPoint source, RouterPoint target, List<uint> path)
    {
      if (this.CustomRouteBuilder != null)
        return this.CustomRouteBuilder(this._db, profile, this.GetGetFactor(profile), source, target, path);
      return CompleteRouteBuilder.TryBuild(this._db, profile, source, target, path);
    }
  }
}
