using OsmSharp.Collections.Tags;
using OsmSharp.Geo;
using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Algorithms.Search;
using OsmSharp.Routing.Graphs.Geometric.Shapes;
using OsmSharp.Routing.Network.Data;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Network
{
  public static class RoutingNetworkExtensions
  {
    public static ICoordinate GetFirstPoint(this RoutingNetwork graph, RoutingEdge edge, uint vertex)
    {
      List<ICoordinate> coordinateList = new List<ICoordinate>();
      if ((int) edge.From == (int) vertex)
      {
        if (edge.Shape == null)
          return (ICoordinate) graph.GetVertex(edge.To);
        IEnumerator<ICoordinate> enumerator = edge.Shape.GetEnumerator();
        enumerator.MoveNext();
        return enumerator.Current;
      }
      if ((int) edge.To == (int) vertex)
      {
        if (edge.Shape == null)
          return (ICoordinate) graph.GetVertex(edge.From);
        IEnumerator<ICoordinate> enumerator = edge.Shape.Reverse().GetEnumerator();
        enumerator.MoveNext();
        return enumerator.Current;
      }
      throw new ArgumentOutOfRangeException(string.Format("Vertex {0} is not part of edge {1}.", new object[2]
      {
        (object) vertex,
        (object) edge.Id
      }));
    }

    public static uint GetOther(this RoutingEdge edge, uint vertex)
    {
      if ((int) edge.From == (int) vertex)
        return edge.To;
      if ((int) edge.To == (int) vertex)
        return edge.From;
      throw new ArgumentOutOfRangeException(string.Format("Vertex {0} is not part of edge {1}.", new object[2]
      {
        (object) vertex,
        (object) edge.Id
      }));
    }

    public static List<ICoordinate> GetShape(this RoutingNetwork graph, RoutingEdge edge)
    {
      List<ICoordinate> coordinateList = new List<ICoordinate>();
      coordinateList.Add((ICoordinate) graph.GetVertex(edge.From));
      ShapeBase shapeBase = edge.Shape;
      if (shapeBase != null)
      {
        if (edge.DataInverted)
          shapeBase = shapeBase.Reverse();
        IEnumerator<ICoordinate> enumerator = shapeBase.GetEnumerator();
        enumerator.Reset();
        while (enumerator.MoveNext())
          coordinateList.Add(enumerator.Current);
      }
      coordinateList.Add((ICoordinate) graph.GetVertex(edge.To));
      return coordinateList;
    }

    public static FeatureCollection GetFeatures(this RoutingNetwork network)
    {
      return network.GetFeaturesIn(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);
    }

    public static FeatureCollection GetFeaturesIn(this RoutingNetwork network, float minLatitude, float minLongitude, float maxLatitude, float maxLongitude)
    {
      FeatureCollection featureCollection = new FeatureCollection();
      HashSet<uint> uintSet = network.GeometricGraph.Search(minLatitude, minLongitude, maxLatitude, maxLongitude);
      HashSet<long> longSet = new HashSet<long>();
      RoutingNetwork.EdgeEnumerator edgeEnumerator = network.GetEdgeEnumerator();
      foreach (uint vertex1 in uintSet)
      {
        GeoCoordinateSimple vertex2 = network.GeometricGraph.GetVertex(vertex1);
        featureCollection.Add(new Feature((Geometry) new Point(new GeoCoordinate((double) vertex2.Latitude, (double) vertex2.Longitude)), (GeometryAttributeCollection) new SimpleGeometryAttributeCollection((IEnumerable<Tag>) new Tag[1]
        {
          Tag.Create("id", vertex1.ToInvariantString())
        })));
        edgeEnumerator.MoveTo(vertex1);
        edgeEnumerator.Reset();
        while (edgeEnumerator.MoveNext())
        {
          if (!longSet.Contains((long) edgeEnumerator.Id))
          {
            longSet.Add((long) edgeEnumerator.Id);
            List<ICoordinate> shape = network.GetShape(edgeEnumerator.Current);
            List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
            foreach (ICoordinate coordinate in shape)
              geoCoordinateList.Add(new GeoCoordinate((double) coordinate.Latitude, (double) coordinate.Longitude));
            LineString lineString = new LineString((IEnumerable<GeoCoordinate>) geoCoordinateList);
            featureCollection.Add(new Feature((Geometry) lineString, (GeometryAttributeCollection) new SimpleGeometryAttributeCollection((IEnumerable<Tag>) new Tag[1]
            {
              Tag.Create("id", edgeEnumerator.Id.ToInvariantString())
            })));
          }
        }
      }
      return featureCollection;
    }

    public static FeatureCollection GetFeaturesFor(this RoutingNetwork network, List<uint> vertices)
    {
      FeatureCollection featureCollection = new FeatureCollection();
      foreach (uint vertex in vertices)
      {
        float latitude;
        float longitude;
        if (network.GeometricGraph.GetVertex(vertex, out latitude, out longitude))
        {
          GeoCoordinate geoCoordinate = new GeoCoordinate((double) latitude, (double) longitude);
          featureCollection.Add(new Feature((Geometry) new Point(new GeoCoordinate(geoCoordinate.Latitude, geoCoordinate.Longitude)), (GeometryAttributeCollection) new SimpleGeometryAttributeCollection((IEnumerable<Tag>) new Tag[1]
          {
            Tag.Create("id", vertex.ToInvariantString())
          })));
        }
      }
      return featureCollection;
    }

    public static bool ContainsEdge(this RoutingNetwork network, uint vertex1, uint vertex2)
    {
      RoutingNetwork.EdgeEnumerator edgeEnumerator = network.GetEdgeEnumerator(vertex1);
      while (edgeEnumerator.MoveNext())
      {
        if ((int) edgeEnumerator.To == (int) vertex2)
          return true;
      }
      return false;
    }

    public static uint AddEdge(this RoutingNetwork network, uint vertex1, uint vertex2, ushort profile, uint metaId, float distance, params ICoordinate[] shape)
    {
      return network.AddEdge(vertex1, vertex2, new EdgeData()
      {
        Distance = distance,
        MetaId = metaId,
        Profile = profile
      }, (ShapeBase) new ShapeEnumerable((IEnumerable<ICoordinate>) shape));
    }

    public static uint AddEdge(this RoutingNetwork network, uint vertex1, uint vertex2, EdgeData data, params ICoordinate[] shape)
    {
      return network.AddEdge(vertex1, vertex2, data, (ShapeBase) new ShapeEnumerable((IEnumerable<ICoordinate>) shape));
    }

    public static uint AddEdge(this RoutingNetwork network, uint vertex1, uint vertex2, EdgeData data, IEnumerable<ICoordinate> shape)
    {
      if (shape == null)
        return network.AddEdge(vertex1, vertex2, data, (ShapeBase) null);
      return network.AddEdge(vertex1, vertex2, data, (ShapeBase) new ShapeEnumerable(shape));
    }

    public static uint AddEdge(this RoutingNetwork network, uint vertex1, uint vertex2, ushort profile, uint metaId, float distance, IEnumerable<ICoordinate> shape)
    {
      return network.AddEdge(vertex1, vertex2, new EdgeData()
      {
        Distance = distance,
        MetaId = metaId,
        Profile = profile
      }, (ShapeBase) new ShapeEnumerable(shape));
    }

    public static void MergeVertices(this RoutingNetwork network, uint vertex1, uint vertex2)
    {
      List<RoutingEdge> routingEdgeList = new List<RoutingEdge>((IEnumerable<RoutingEdge>) network.GetEdgeEnumerator(vertex2));
      network.RemoveEdges(vertex2);
      for (int index = 0; index < routingEdgeList.Count; ++index)
      {
        if (!routingEdgeList[index].DataInverted)
        {
          int num1 = (int) network.AddEdge(vertex1, routingEdgeList[index].To, routingEdgeList[index].Data, routingEdgeList[index].Shape);
        }
        else
        {
          int num2 = (int) network.AddEdge(routingEdgeList[index].To, vertex1, routingEdgeList[index].Data, routingEdgeList[index].Shape);
        }
      }
    }
  }
}
