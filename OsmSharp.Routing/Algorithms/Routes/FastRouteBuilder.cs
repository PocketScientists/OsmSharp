using OsmSharp.Collections.Tags;
using OsmSharp.Geo;
using OsmSharp.Routing.Graphs.Geometric.Shapes;
using OsmSharp.Routing.Network;
using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Routing.Algorithms.Routes
{
  public class FastRouteBuilder : AlgorithmBase
  {
    private readonly RouterDb _routerDb;
    private readonly List<uint> _path;
    private readonly Profile _profile;
    private readonly Func<ushort, Factor> _getFactor;
    private readonly RouterPoint _source;
    private readonly RouterPoint _target;
    private Route _route;
    private TagsCollection _empty;

    public Route Route
    {
      get
      {
        return this._route;
      }
    }

    public FastRouteBuilder(RouterDb routerDb, Profile profile, Func<ushort, Factor> getFactor, RouterPoint source, RouterPoint target, List<uint> path)
    {
      this._routerDb = routerDb;
      this._path = path;
      this._source = source;
      this._target = target;
      this._profile = profile;
      this._getFactor = getFactor;
    }

    protected override void DoRun()
    {
      this._empty = new TagsCollection();
      if (this._path.Count == 0)
      {
        this.ErrorMessage = "Path was empty.";
        this.HasSucceeded = false;
      }
      else
      {
        uint vertex1 = this._path[0];
        if ((int) vertex1 != -2 && !this._source.IsVertex(this._routerDb, vertex1))
        {
          this.ErrorMessage = "The source is a vertex but the source is not a match.";
          this.HasSucceeded = false;
        }
        else
        {
          uint vertex2 = this._path[this._path.Count - 1];
          if ((int) vertex2 != -2 && !this._target.IsVertex(this._routerDb, vertex2))
          {
            this.ErrorMessage = "The target is a vertex but the target is not a match.";
            this.HasSucceeded = false;
          }
          else
          {
            this._route = new Route();
            this._route.Segments = new List<RouteSegment>();
            this.AddSource();
            if (this._path.Count == 1)
            {
              this.HasSucceeded = true;
            }
            else
            {
              int index;
              for (index = 0; index < this._path.Count - 2; ++index)
                this.Add(this._path[index], this._path[index + 1], this._path[index + 2]);
              this.Add(this._path[index], this._path[index + 1]);
              this.HasSucceeded = true;
            }
            if (this._route.Segments.Count == 1)
            {
              this._route.Segments[0].SetStop(new ICoordinate[2]
              {
                this._source.Location(),
                this._target.Location()
              }, new TagsCollectionBase[2]
              {
                this._source.Tags,
                this._target.Tags
              });
            }
            else
            {
              this._route.Segments[0].SetStop(this._source.Location(), this._source.Tags);
              this._route.Segments[this._route.Segments.Count - 1].SetStop(this._target.Location(), this._target.Tags);
              this._route.TotalDistance = this._route.Segments[this._route.Segments.Count - 1].Distance;
              this._route.TotalTime = this._route.Segments[this._route.Segments.Count - 1].Time;
            }
          }
        }
      }
    }

    private void AddSource()
    {
      this._route.Segments.Add(RouteSegment.CreateNew(this._source.Location(), this._profile));
    }

    private void Add(uint from, uint to)
    {
      if ((int) from == -2 && this._source.IsVertex())
        from = this._source.VertexId(this._routerDb);
      if ((int) to == -2 && this._target.IsVertex())
        to = this._target.VertexId(this._routerDb);
      List<ICoordinate> coordinateList = new List<ICoordinate>(0);
      RoutingEdge edge;
      ICoordinate coordinate;
      if ((int) from == -2 && (int) to == -2)
      {
        if ((int) this._source.EdgeId != (int) this._target.EdgeId)
        {
          this.ErrorMessage = "Target and source have to be on the same vertex with a route with only virtual vertices.";
          return;
        }
        coordinateList = this._source.ShapePointsTo(this._routerDb, this._target);
        edge = this._routerDb.Network.GetEdge(this._source.EdgeId);
        coordinate = this._target.Location();
      }
      else if ((int) from == -2)
      {
        edge = this._routerDb.Network.GetEdge(this._source.EdgeId);
        coordinateList = this._source.ShapePointsTo(this._routerDb, this._routerDb.Network.CreateRouterPointForVertex(to, edge.GetOther(to)));
        coordinate = (ICoordinate) this._routerDb.Network.GetVertex(to);
      }
      else if ((int) to == -2)
      {
        edge = this._routerDb.Network.GetEdge(this._target.EdgeId);
        coordinateList = this._routerDb.Network.CreateRouterPointForVertex(from, edge.GetOther(from)).ShapePointsTo(this._routerDb, this._target);
        coordinate = this._target.Location();
      }
      else
      {
        edge = this._routerDb.Network.GetEdgeEnumerator(from).First<RoutingEdge>((Func<RoutingEdge, bool>) (x => (int) x.To == (int) to));
        ShapeBase shapeBase = edge.Shape;
        if (shapeBase != null)
        {
          if (edge.DataInverted)
            shapeBase = shapeBase.Reverse();
          coordinateList.AddRange((IEnumerable<ICoordinate>) shapeBase);
        }
        coordinate = (ICoordinate) this._routerDb.Network.GetVertex(to);
      }
      Speed speedFor = this.GetSpeedFor(edge.Data.Profile);
      for (int index = 0; index < coordinateList.Count; ++index)
      {
        RouteSegment segment = RouteSegment.CreateNew(coordinateList[index], this._profile);
        segment.Set(this._route.Segments[this._route.Segments.Count - 1], this._profile, (TagsCollectionBase) this._empty, speedFor);
        this._route.Segments.Add(segment);
      }
      RouteSegment segment1 = RouteSegment.CreateNew(coordinate, this._profile);
      segment1.Set(this._route.Segments[this._route.Segments.Count - 1], this._profile, (TagsCollectionBase) this._empty, speedFor);
      this._route.Segments.Add(segment1);
    }

    private void Add(uint from, uint to, uint next)
    {
      if ((int) from == -2 && this._source.IsVertex())
        from = this._source.VertexId(this._routerDb);
      if ((int) next == -2 && this._target.IsVertex())
        next = this._target.VertexId(this._routerDb);
      List<ICoordinate> coordinateList = new List<ICoordinate>(0);
      RoutingEdge edge;
      ICoordinate coordinate;
      if ((int) from == -2 && (int) to == -2)
      {
        if ((int) this._source.EdgeId != (int) this._target.EdgeId)
        {
          this.ErrorMessage = "Target and source have to be on the same vertex with a route with only virtual vertices.";
          return;
        }
        coordinateList = this._source.ShapePointsTo(this._routerDb, this._target);
        edge = this._routerDb.Network.GetEdge(this._source.EdgeId);
        coordinate = this._target.Location();
      }
      else if ((int) from == -2)
      {
        edge = this._routerDb.Network.GetEdge(this._source.EdgeId);
        coordinateList = this._source.ShapePointsTo(this._routerDb, this._routerDb.Network.CreateRouterPointForVertex(to, edge.GetOther(to)));
        coordinate = (ICoordinate) this._routerDb.Network.GetVertex(to);
      }
      else if ((int) to == -2)
      {
        edge = this._routerDb.Network.GetEdge(this._target.EdgeId);
        coordinateList = this._routerDb.Network.CreateRouterPointForVertex(from, edge.GetOther(from)).ShapePointsTo(this._routerDb, this._target);
        coordinate = this._target.Location();
      }
      else
      {
        edge = this._routerDb.Network.GetEdgeEnumerator(from).First<RoutingEdge>((Func<RoutingEdge, bool>) (x => (int) x.To == (int) to));
        ShapeBase shapeBase = edge.Shape;
        if (shapeBase != null)
        {
          if (edge.DataInverted)
            shapeBase = shapeBase.Reverse();
          coordinateList.AddRange((IEnumerable<ICoordinate>) shapeBase);
        }
        coordinate = (ICoordinate) this._routerDb.Network.GetVertex(to);
      }
      Speed speedFor = this.GetSpeedFor(edge.Data.Profile);
      for (int index = 0; index < coordinateList.Count; ++index)
      {
        RouteSegment segment = RouteSegment.CreateNew(coordinateList[index], this._profile);
        segment.Set(this._route.Segments[this._route.Segments.Count - 1], this._profile, (TagsCollectionBase) this._empty, speedFor);
        this._route.Segments.Add(segment);
      }
      RouteSegment segment1 = RouteSegment.CreateNew(coordinate, this._profile);
      segment1.Set(this._route.Segments[this._route.Segments.Count - 1], this._profile, (TagsCollectionBase) this._empty, speedFor);
      this._route.Segments.Add(segment1);
    }

    private Speed GetSpeedFor(ushort profileId)
    {
      Speed speed = new Speed()
      {
        Direction = 0,
        Value = 1f
      };
      if (this._profile.Metric == ProfileMetric.TimeInSeconds)
      {
        float num = this._getFactor(profileId).Value;
        if ((double) num != 0.0)
          speed = new Speed()
          {
            Direction = (short) 0,
            Value = 1f / num
          };
      }
      else
        speed = this._profile.Speed(this._routerDb.EdgeProfiles.Get((uint) profileId));
      return speed;
    }

    public static Route Build(RouterDb db, Profile profile, Func<ushort, Factor> getFactor, RouterPoint source, RouterPoint target, Path path)
    {
      return FastRouteBuilder.TryBuild(db, profile, getFactor, source, target, path).Value;
    }

    public static Result<Route> TryBuild(RouterDb db, Profile profile, Func<ushort, Factor> getFactor, RouterPoint source, RouterPoint target, Path path)
    {
      List<uint> uintList = new List<uint>();
      path.AddToList(uintList);
      return FastRouteBuilder.TryBuild(db, profile, getFactor, source, target, uintList);
    }

    public static Route Build(RouterDb db, Profile profile, Func<ushort, Factor> getFactor, RouterPoint source, RouterPoint target, List<uint> path)
    {
      return FastRouteBuilder.TryBuild(db, profile, getFactor, source, target, path).Value;
    }

    public static Result<Route> TryBuild(RouterDb db, Profile profile, Func<ushort, Factor> getFactor, RouterPoint source, RouterPoint target, List<uint> path)
    {
      FastRouteBuilder fastRouteBuilder = new FastRouteBuilder(db, profile, getFactor, source, target, path);
      fastRouteBuilder.Run();
      if (fastRouteBuilder.HasSucceeded)
        return new Result<Route>(fastRouteBuilder.Route);
      return new Result<Route>(string.Format("Failed to build route: {0}", (object) fastRouteBuilder.ErrorMessage));
    }
  }
}
