using OsmSharp.Geo;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Algorithms;
using OsmSharp.Routing.Data;
using OsmSharp.Routing.Graphs.Geometric;
using OsmSharp.Routing.Network;
using OsmSharp.Routing.Profiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing
{
  public static class RouterPointExtensions
  {
    public static Path[] ToPaths(this RouterPoint point, RouterDb routerDb, Func<ushort, Factor> getFactor, bool asSource)
    {
      GeometricEdge edge = routerDb.Network.GeometricGraph.GetEdge(point.EdgeId);
      float distance;
      ushort profile;
      EdgeDataSerializer.Deserialize(edge.Data[0], out distance, out profile);
      Factor factor = getFactor(profile);
      float num1 = distance;
      float num2 = (float) point.Offset / (float) ushort.MaxValue;
      if ((int) factor.Direction == 0)
      {
        if ((double) num2 == 0.0)
          return new Path[2]
          {
            new Path(edge.From),
            new Path(edge.To, num1 * (1f - num2) * factor.Value, new Path(edge.From))
          };
        if ((double) num2 == 1.0)
          return new Path[2]
          {
            new Path(edge.From, num1 * num2 * factor.Value, new Path(edge.To)),
            new Path(edge.To)
          };
        return new Path[2]
        {
          new Path(edge.From, num1 * num2 * factor.Value, new Path(4294967294U)),
          new Path(edge.To, num1 * (1f - num2) * factor.Value, new Path(4294967294U))
        };
      }
      if ((int) factor.Direction == 1)
      {
        if (asSource)
        {
          if ((double) num2 == 1.0)
            return new Path[1]
            {
              new Path(edge.To)
            };
          if ((double) num2 == 0.0)
            return new Path[2]
            {
              new Path(edge.From),
              new Path(edge.To, num1 * (1f - num2) * factor.Value, new Path(edge.From))
            };
          return new Path[1]
          {
            new Path(edge.To, num1 * (1f - num2) * factor.Value, new Path(4294967294U))
          };
        }
        if ((double) num2 == 0.0)
          return new Path[1]
          {
            new Path(edge.From)
          };
        if ((double) num2 == 1.0)
          return new Path[2]
          {
            new Path(edge.To),
            new Path(edge.From, num1 * num2 * factor.Value, new Path(edge.To))
          };
        return new Path[1]
        {
          new Path(edge.From, num1 * num2 * factor.Value, new Path(4294967294U))
        };
      }
      if (!asSource)
      {
        if ((double) num2 == 1.0)
          return new Path[1]
          {
            new Path(edge.To)
          };
        if ((double) num2 == 0.0)
          return new Path[2]
          {
            new Path(edge.From),
            new Path(edge.To, num1 * (1f - num2) * factor.Value, new Path(edge.From))
          };
        return new Path[1]
        {
          new Path(edge.To, num1 * (1f - num2) * factor.Value, new Path(4294967294U))
        };
      }
      if ((double) num2 == 0.0)
        return new Path[1]
        {
          new Path(edge.From)
        };
      if ((double) num2 == 1.0)
        return new Path[2]
        {
          new Path(edge.To),
          new Path(edge.From, num1 * num2 * factor.Value, new Path(edge.To))
        };
      return new Path[1]
      {
        new Path(edge.From, num1 * num2 * factor.Value, new Path(4294967294U))
      };
    }

    public static Path[] ToPaths(this RouterPoint point, RouterDb routerDb, Profile profile, bool asSource)
    {
      return point.ToPaths(routerDb, (Func<ushort, Factor>) (p => profile.Factor(routerDb.EdgeProfiles.Get((uint) p))), asSource);
    }

    public static float DistanceTo(this RouterPoint point, RouterDb routerDb, uint vertex)
    {
      GeometricEdge edge = routerDb.Network.GeometricGraph.GetEdge(point.EdgeId);
      float num1 = routerDb.Network.GeometricGraph.Length(edge);
      float num2 = num1 * ((float) point.Offset / (float) ushort.MaxValue);
      if ((int) edge.From == (int) vertex)
        return num2;
      if ((int) edge.To == (int) vertex)
        return num1 - num2;
      throw new ArgumentOutOfRangeException(string.Format("Vertex {0} is not part of edge {1}.", new object[2]
      {
        (object) vertex,
        (object) point.EdgeId
      }));
    }

    public static List<ICoordinate> ShapePointsTo(this RouterPoint point, RouterDb routerDb, uint vertex)
    {
      GeometricEdge edge = routerDb.Network.GeometricGraph.GetEdge(point.EdgeId);
      float num = routerDb.Network.GeometricGraph.Length(edge);
      float maxDistance = num * ((float) point.Offset / (float) ushort.MaxValue);
      if ((int) edge.From == (int) vertex)
      {
        List<ICoordinate> shape = routerDb.Network.GeometricGraph.GetShape(edge, vertex, maxDistance);
        shape.Reverse();
        return shape;
      }
      if ((int) edge.To == (int) vertex)
      {
        List<ICoordinate> shape = routerDb.Network.GeometricGraph.GetShape(edge, vertex, num - maxDistance);
        shape.Reverse();
        return shape;
      }
      throw new ArgumentOutOfRangeException(string.Format("Vertex {0} is not part of edge {1}.", new object[2]
      {
        (object) vertex,
        (object) point.EdgeId
      }));
    }

    public static List<ICoordinate> ShapePointsTo(this RouterPoint point, RouterDb routerDb, RouterPoint other)
    {
      if ((int) point.EdgeId != (int) other.EdgeId)
        throw new ArgumentException("Cannot build shape points list between router points on different edges.");
      GeometricEdge edge = routerDb.Network.GeometricGraph.GetEdge(point.EdgeId);
      double num1 = (double) routerDb.Network.GeometricGraph.Length(edge);
      double num2 = (double) point.Offset / (double) ushort.MaxValue;
      float num3 = (float) (num1 * num2);
      double num4 = (double) other.Offset / (double) ushort.MaxValue;
      float num5 = (float) (num1 * num4);
      List<ICoordinate> shape;
      if ((double) num3 > (double) num5)
      {
        shape = routerDb.Network.GeometricGraph.GetShape(edge, num5, num3);
        shape.Reverse();
      }
      else
        shape = routerDb.Network.GeometricGraph.GetShape(edge, num3, num5);
      return shape;
    }

    public static ICoordinate Location(this RouterPoint point)
    {
      return (ICoordinate) new GeoCoordinateSimple()
      {
        Latitude = point.Latitude,
        Longitude = point.Longitude
      };
    }

    public static ICoordinate LocationOnNetwork(this RouterPoint point, RouterDb db)
    {
      GeometricEdge edge = db.Network.GeometricGraph.GetEdge(point.EdgeId);
      List<ICoordinate> shape = db.Network.GeometricGraph.GetShape(edge);
      double num1 = (double) db.Network.GeometricGraph.Length(edge);
      double num2 = 0.0;
      double num3 = num1 * ((double) point.Offset / (double) ushort.MaxValue);
      for (int index = 1; index < shape.Count; ++index)
      {
        double num4 = GeoCoordinate.DistanceEstimateInMeter(shape[index - 1], shape[index]);
        if (num4 + num2 > num3)
        {
          double num5 = 1.0 - (num4 + num2 - num3) / num4;
          return (ICoordinate) new GeoCoordinateSimple()
          {
            Latitude = (shape[index - 1].Latitude + (float) (num5 * ((double) shape[index].Latitude - (double) shape[index - 1].Latitude))),
            Longitude = (shape[index - 1].Longitude + (float) (num5 * ((double) shape[index].Longitude - (double) shape[index - 1].Longitude)))
          };
        }
        num2 += num4;
      }
      return shape[shape.Count - 1];
    }

    public static bool IsVertex(this RouterPoint point)
    {
      return (int) point.Offset == 0 || (int) point.Offset == (int) ushort.MaxValue;
    }

    public static bool IsVertex(this RouterPoint point, RouterDb router, uint vertex)
    {
      if ((int) point.Offset == 0)
        return (int) router.Network.GetEdge(point.EdgeId).From == (int) vertex;
      if ((int) point.Offset == (int) ushort.MaxValue)
        return (int) router.Network.GetEdge(point.EdgeId).To == (int) vertex;
      return false;
    }

    public static RouterPoint CreateRouterPointForVertex(this RoutingNetwork graph, uint vertex)
    {
      return graph.GeometricGraph.CreateRouterPointForVertex(vertex);
    }

    public static RouterPoint CreateRouterPointForVertex(this GeometricGraph graph, uint vertex)
    {
      float latitude;
      float longitude;
      if (!graph.GetVertex(vertex, out latitude, out longitude))
        throw new ArgumentException("Vertex doesn't exist, cannot create routerpoint.");
      GeometricGraph.EdgeEnumerator edgeEnumerator = graph.GetEdgeEnumerator(vertex);
      if (!edgeEnumerator.MoveNext())
        throw new ArgumentException("No edges associated with vertex, cannot create routerpoint.");
      if (edgeEnumerator.DataInverted)
        return new RouterPoint(latitude, longitude, edgeEnumerator.Id, ushort.MaxValue);
      return new RouterPoint(latitude, longitude, edgeEnumerator.Id, (ushort) 0);
    }

    public static RouterPoint CreateRouterPointForVertex(this RoutingNetwork graph, uint vertex, uint neighbour)
    {
      return graph.GeometricGraph.CreateRouterPointForVertex(vertex, neighbour);
    }

    public static RouterPoint CreateRouterPointForVertex(this GeometricGraph graph, uint vertex, uint neighbour)
    {
      float latitude;
      float longitude;
      if (!graph.GetVertex(vertex, out latitude, out longitude))
        throw new ArgumentException("Vertex doesn't exist, cannot create routerpoint.");
      GeometricGraph.EdgeEnumerator edgeEnumerator = graph.GetEdgeEnumerator(vertex);
      while (edgeEnumerator.MoveNext())
      {
        if ((int) edgeEnumerator.To == (int) neighbour)
        {
          if (edgeEnumerator.DataInverted)
            return new RouterPoint(latitude, longitude, edgeEnumerator.Id, ushort.MaxValue);
          return new RouterPoint(latitude, longitude, edgeEnumerator.Id, (ushort) 0);
        }
      }
      throw new ArgumentException("No edges associated with vertex and it's neigbour, cannot create routerpoint.");
    }

    public static Path PathTo(this RouterPoint point, RouterDb db, Profile profile, RouterPoint target)
    {
      return point.PathTo(db, (Func<ushort, Factor>) (p => profile.Factor(db.EdgeProfiles.Get((uint) p))), target);
    }

    public static Path PathTo(this RouterPoint point, RouterDb db, Func<ushort, Factor> getFactor, RouterPoint target)
    {
      if ((int) point.EdgeId != (int) target.EdgeId)
        throw new ArgumentException("Target point must be part of the same edge.");
      if ((int) point.Offset == (int) target.Offset)
        return new Path(point.VertexId(db));
      bool flag = (int) point.Offset < (int) target.Offset;
      RoutingEdge edge = db.Network.GetEdge(point.EdgeId);
      Factor factor = getFactor(edge.Data.Profile);
      if ((double) factor.Value <= 0.0)
        return (Path) null;
      if ((int) factor.Direction != 0 && (!flag || (int) factor.Direction != 1) && (flag || (int) factor.Direction != 2))
        return (Path) null;
      float weight = (float) System.Math.Abs((int) point.Offset - (int) target.Offset) / (float) ushort.MaxValue * edge.Data.Distance * factor.Value;
      return new Path(target.VertexId(db), weight, new Path(point.VertexId(db)));
    }

    public static uint VertexId(this RouterPoint point, RouterDb db)
    {
      RoutingEdge edge = db.Network.GetEdge(point.EdgeId);
      if ((int) point.Offset == 0)
        return edge.From;
      if ((int) point.Offset == (int) ushort.MaxValue)
        return edge.To;
      return 4294967294;
    }

    public static bool IsIdenticalTo(this RouterPoint point, RouterPoint other)
    {
      if ((int) other.EdgeId == (int) point.EdgeId)
        return (int) other.Offset == (int) point.Offset;
      return false;
    }
  }
}
