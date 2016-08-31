using OsmSharp.Collections.Tags;
using OsmSharp.Geo;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Network;
using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms
{
  public class WeightMatrixAlgorithm : AlgorithmBase, IWeightMatrixAlgorithm, IAlgorithm
  {
    private readonly IRouter _router;
    private readonly Profile _profile;
    private readonly GeoCoordinate[] _locations;
    private readonly Func<RoutingEdge, int, bool> _matchEdge;
    private Dictionary<int, LocationError> _errors;
    private List<int> _resolvedPointsIndices;
    private List<RouterPoint> _resolvedPoints;
    private float[][] _weights;

    public float SearchDistanceInMeter { get; set; }

    public List<RouterPoint> RouterPoints
    {
      get
      {
        this.CheckHasRunAndHasSucceeded();
        return this._resolvedPoints;
      }
    }

    public float[][] Weights
    {
      get
      {
        this.CheckHasRunAndHasSucceeded();
        return this._weights;
      }
    }

    public Dictionary<int, LocationError> Errors
    {
      get
      {
        this.CheckHasRunAndHasSucceeded();
        return this._errors;
      }
    }

    public WeightMatrixAlgorithm(IRouter router, Profile profile, GeoCoordinate[] locations)
      : this(router, profile, locations, (Func<RoutingEdge, int, bool>) null)
    {
    }

    public WeightMatrixAlgorithm(IRouter router, Profile profile, GeoCoordinate[] locations, Func<RoutingEdge, int, bool> matchEdge)
    {
      this._router = router;
      this._profile = profile;
      this._locations = locations;
      this._matchEdge = matchEdge;
      this.SearchDistanceInMeter = 50f;
    }

    protected override void DoRun()
    {
      this._errors = new Dictionary<int, LocationError>(this._locations.Length);
      this._resolvedPoints = new List<RouterPoint>(this._locations.Length);
      this._resolvedPointsIndices = new List<int>(this._locations.Length);
      RouterPoint[] routerPointArray = new RouterPoint[this._locations.Length];
      Profile[] profiles = new Profile[1]
      {
        this._profile
      };
      int num;
      for (int i = 0; i < this._locations.Length; i = num + 1)
      {
        Result<RouterPoint> result = this._matchEdge == null ? this._router.TryResolve(profiles, (ICoordinate) this._locations[i], this.SearchDistanceInMeter) : this._router.TryResolve(profiles, (ICoordinate) this._locations[i], (Func<RoutingEdge, bool>) (edge => this._matchEdge(edge, i)), this.SearchDistanceInMeter);
        if (!result.IsError)
          routerPointArray[i] = result.Value;
        num = i;
      }
      for (int index = 0; index < routerPointArray.Length; ++index)
      {
        if (routerPointArray[index] == null)
        {
          this._errors[index] = new LocationError()
          {
            Code = LocationErrorCode.NotResolved,
            Message = "Location could not be linked to the road network."
          };
        }
        else
        {
          routerPointArray[index].Tags.Add(new Tag("index", index.ToInvariantString()));
          this._resolvedPointsIndices.Add(index);
          this._resolvedPoints.Add(routerPointArray[index]);
        }
      }
      RouterPoint[] array = this._resolvedPoints.ToArray();
      HashSet<int> toRemove = new HashSet<int>();
      this._weights = this._router.CalculateWeight(this._profile, array, (ISet<int>) toRemove);
      if (toRemove.Count > 0)
      {
        foreach (int index in toRemove)
          this._errors[this._resolvedPointsIndices[index]] = new LocationError()
          {
            Code = LocationErrorCode.NotRoutable,
            Message = "Location could not routed to or from."
          };
        this._resolvedPoints = this._resolvedPoints.ShrinkAndCopyList<RouterPoint>(toRemove);
        this._resolvedPointsIndices = this._resolvedPointsIndices.ShrinkAndCopyList<int>(toRemove);
        this._weights = this._weights.SchrinkAndCopyMatrix<float>(toRemove);
      }
      this.HasSucceeded = true;
    }

    public int IndexOf(int locationIdx)
    {
      this.CheckHasRunAndHasSucceeded();
      return this._resolvedPointsIndices.IndexOf(locationIdx);
    }

    public int LocationIndexOf(int routerPointIdx)
    {
      this.CheckHasRunAndHasSucceeded();
      return this._resolvedPointsIndices[routerPointIdx];
    }
  }
}
