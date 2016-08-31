using OsmSharp.Collections.Tags;
using OsmSharp.Geo;
using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.Algorithms.Search;
using OsmSharp.Routing.Graphs.Geometric.Shapes;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Graphs.Geometric
{
  public static class GeometricExtensions
  {
    public static bool ProjectOn(this GeometricGraph graph, GeometricEdge edge, float latitude, float longitude, out float projectedLatitude, out float projectedLongitude, out float projectedDistanceFromFirst)
    {
      int projectedShapeIndex;
      float distanceToProjected;
      return graph.ProjectOn(edge, latitude, longitude, out projectedLatitude, out projectedLongitude, out projectedDistanceFromFirst, out projectedShapeIndex, out distanceToProjected);
    }

    public static bool ProjectOn(this GeometricGraph graph, GeometricEdge edge, float latitude, float longitude, out float projectedLatitude, out float projectedLongitude, out float projectedDistanceFromFirst, out int projectedShapeIndex, out float distanceToProjected)
    {
      float totalLength;
      return graph.ProjectOn(edge, latitude, longitude, out projectedLatitude, out projectedLongitude, out projectedDistanceFromFirst, out projectedShapeIndex, out distanceToProjected, out totalLength);
    }

    public static bool ProjectOn(this GeometricGraph graph, GeometricEdge edge, float latitude, float longitude, out float projectedLatitude, out float projectedLongitude, out float projectedDistanceFromFirst, out int projectedShapeIndex, out float distanceToProjected, out float totalLength)
    {
      distanceToProjected = float.MaxValue;
      projectedDistanceFromFirst = 0.0f;
      projectedLatitude = float.MaxValue;
      projectedLongitude = float.MaxValue;
      projectedShapeIndex = -1;
      ICoordinate coordinate1 = (ICoordinate) graph.GetVertex(edge.From);
      ShapeBase shape = edge.Shape;
      IEnumerator<ICoordinate> enumerator = (IEnumerator<ICoordinate>) null;
      if (shape != null)
      {
        enumerator = shape.GetEnumerator();
        enumerator.Reset();
      }
      float num1 = 0.0f;
      int num2 = -1;
      while (true)
      {
        bool flag = true;
        ICoordinate coordinate2;
        if (enumerator != null && enumerator.MoveNext())
        {
          coordinate2 = enumerator.Current;
        }
        else
        {
          flag = false;
          coordinate2 = (ICoordinate) graph.GetVertex(edge.To);
        }
        PointF2D pointF2D = new GeoCoordinateLine(new GeoCoordinate((double) coordinate1.Latitude, (double) coordinate1.Longitude), new GeoCoordinate((double) coordinate2.Latitude, (double) coordinate2.Longitude), true, true).ProjectOn((PointF2D) new GeoCoordinate((double) latitude, (double) longitude));
        if (pointF2D != (PointF2D) null)
        {
          double num3 = GeoCoordinate.DistanceEstimateInMeter(pointF2D[1], pointF2D[0], (double) latitude, (double) longitude);
          if (num3 < (double) distanceToProjected)
          {
            distanceToProjected = (float) num3;
            projectedLatitude = (float) pointF2D[1];
            projectedLongitude = (float) pointF2D[0];
            projectedDistanceFromFirst = num1 + (float) GeoCoordinate.DistanceEstimateInMeter((double) projectedLatitude, (double) projectedLongitude, (double) coordinate1.Latitude, (double) coordinate1.Longitude);
            projectedShapeIndex = num2 + 1;
          }
        }
        if (flag)
        {
          num1 += (float) GeoCoordinate.DistanceEstimateInMeter((double) coordinate1.Latitude, (double) coordinate1.Longitude, (double) coordinate2.Latitude, (double) coordinate2.Longitude);
          ++num2;
          coordinate1 = coordinate2;
        }
        else
          break;
      }
      GeoCoordinateSimple vertex = graph.GetVertex(edge.To);
      totalLength = num1 + (float) GeoCoordinate.DistanceEstimateInMeter((double) coordinate1.Latitude, (double) coordinate1.Longitude, (double) vertex.Latitude, (double) vertex.Longitude);
      return (double) distanceToProjected != 3.40282346638529E+38;
    }

    public static float Length(this GeometricGraph graph, GeometricEdge edge)
    {
      float num = 0.0f;
      ICoordinate coordinate = (ICoordinate) graph.GetVertex(edge.From);
      ShapeBase shape = edge.Shape;
      if (shape != null)
      {
        IEnumerator<ICoordinate> enumerator = shape.GetEnumerator();
        enumerator.Reset();
        while (enumerator.MoveNext())
        {
          ICoordinate current = enumerator.Current;
          num += (float) GeoCoordinate.DistanceEstimateInMeter((double) coordinate.Latitude, (double) coordinate.Longitude, (double) current.Latitude, (double) current.Longitude);
          coordinate = current;
        }
      }
      ICoordinate vertex = (ICoordinate) graph.GetVertex(edge.To);
      return num + (float) GeoCoordinate.DistanceEstimateInMeter((double) coordinate.Latitude, (double) coordinate.Longitude, (double) vertex.Latitude, (double) vertex.Longitude);
    }

    public static List<ICoordinate> GetShape(this GeometricGraph graph, GeometricEdge geometricEdge)
    {
      List<ICoordinate> coordinateList = new List<ICoordinate>();
      coordinateList.Add((ICoordinate) graph.GetVertex(geometricEdge.From));
      ShapeBase shapeBase = geometricEdge.Shape;
      if (shapeBase != null)
      {
        if (geometricEdge.DataInverted)
          shapeBase = shapeBase.Reverse();
        IEnumerator<ICoordinate> enumerator = shapeBase.GetEnumerator();
        enumerator.Reset();
        while (enumerator.MoveNext())
          coordinateList.Add(enumerator.Current);
      }
      coordinateList.Add((ICoordinate) graph.GetVertex(geometricEdge.To));
      return coordinateList;
    }

    public static List<ICoordinate> GetShape(this GeometricGraph graph, GeometricEdge geometricEdge, float minDistance, float maxDistance)
    {
      List<ICoordinate> coordinateList = new List<ICoordinate>();
      if (geometricEdge.Shape == null)
        return coordinateList;
      ICoordinate location1 = (ICoordinate) graph.GetVertex(geometricEdge.From);
      float num = 0.0f;
      IEnumerator<ICoordinate> enumerator = geometricEdge.Shape.GetEnumerator();
      enumerator.Reset();
      while (enumerator.MoveNext())
      {
        ICoordinate current = enumerator.Current;
        num += (float) GeoCoordinate.DistanceEstimateInMeter(location1, current);
        if ((double) minDistance < (double) num && (double) num < (double) maxDistance)
          coordinateList.Add(current);
        location1 = current;
      }
      return coordinateList;
    }

    public static List<ICoordinate> GetShape(this GeometricGraph graph, GeometricEdge geometricEdge, uint vertex, float maxDistance)
    {
      List<ICoordinate> coordinateList = new List<ICoordinate>();
      if (geometricEdge.Shape == null)
        return coordinateList;
      if ((int) geometricEdge.From == (int) vertex)
      {
        ICoordinate location1 = (ICoordinate) graph.GetVertex(vertex);
        float num = 0.0f;
        IEnumerator<ICoordinate> enumerator = geometricEdge.Shape.GetEnumerator();
        while (enumerator.MoveNext())
        {
          ICoordinate current = enumerator.Current;
          num += (float) GeoCoordinate.DistanceEstimateInMeter(location1, current);
          if ((double) num < (double) maxDistance)
          {
            coordinateList.Add(current);
            location1 = current;
          }
          else
            break;
        }
        return coordinateList;
      }
      if ((int) geometricEdge.To == (int) vertex)
      {
        ShapeBase shapeBase = geometricEdge.Shape.Reverse();
        ICoordinate location1 = (ICoordinate) graph.GetVertex(vertex);
        float num = 0.0f;
        IEnumerator<ICoordinate> enumerator = shapeBase.GetEnumerator();
        while (enumerator.MoveNext())
        {
          ICoordinate current = enumerator.Current;
          num += (float) GeoCoordinate.DistanceEstimateInMeter(location1, current);
          if ((double) num < (double) maxDistance)
          {
            coordinateList.Add(current);
            location1 = current;
          }
          else
            break;
        }
        return coordinateList;
      }
      throw new ArgumentOutOfRangeException(string.Format("Vertex {0} is not part of edge {1}.", new object[2]
      {
        (object) vertex,
        (object) geometricEdge.Id
      }));
    }

    public static FeatureCollection GetFeaturesIn(this GeometricGraph graph, float minLatitude, float minLongitude, float maxLatitude, float maxLongitude)
    {
      FeatureCollection featureCollection = new FeatureCollection();
      HashSet<uint> uintSet = graph.Search(minLatitude, minLongitude, maxLatitude, maxLongitude);
      HashSet<long> longSet = new HashSet<long>();
      GeometricGraph.EdgeEnumerator edgeEnumerator = graph.GetEdgeEnumerator();
      foreach (uint vertex1 in uintSet)
      {
        GeoCoordinateSimple vertex2 = graph.GetVertex(vertex1);
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
            List<ICoordinate> shape = graph.GetShape(edgeEnumerator.Current);
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

    public static uint GetOther(this GeometricEdge edge, uint vertex)
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

    public static uint AddEdge(this GeometricGraph graph, uint vertex1, uint vertex2, uint[] data, params ICoordinate[] shape)
    {
      return graph.AddEdge(vertex1, vertex2, data, (ShapeBase) new ShapeEnumerable((IEnumerable<ICoordinate>) shape));
    }

    public static uint AddEdge(this GeometricGraph graph, uint vertex1, uint vertex2, uint[] data, IEnumerable<ICoordinate> shape)
    {
      return graph.AddEdge(vertex1, vertex2, data, (ShapeBase) new ShapeEnumerable(shape));
    }
  }
}
