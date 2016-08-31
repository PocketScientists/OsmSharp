using OsmSharp.Collections.Tags;
using OsmSharp.Geo;
using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.Geo.Streams.GeoJson;
using OsmSharp.Math;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Meta;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Math.Primitives;
using OsmSharp.Routing.Algorithms.Routes;
using OsmSharp.Routing.IO.Gpx;
using OsmSharp.Routing.Network;
using OsmSharp.Routing.Network.Data;
using OsmSharp.Routing.Profiles;
using OsmSharp.Units.Distance;
using OsmSharp.Units.Time;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace OsmSharp.Routing
{
  public static class RouteExtensions
  {
    public static void Save(this Route route, Stream stream)
    {
      new XmlSerializer(typeof (Route)).Serialize(stream, (object) route);
      stream.Flush();
    }

    public static byte[] SaveToByteArray(this Route route)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        route.Save((Stream) memoryStream);
        return memoryStream.ToArray();
      }
    }

    public static Route Load(Stream stream)
    {
      return new XmlSerializer(typeof (Route)).Deserialize(stream) as Route;
    }

    public static Route Load(byte[] bytes)
    {
      using (MemoryStream memoryStream = new MemoryStream(bytes))
        return new XmlSerializer(typeof (Route)).Deserialize((Stream) memoryStream) as Route;
    }

    public static void SaveAsGeoJson(this Route route, Stream stream)
    {
      StreamWriter streamWriter = new StreamWriter(stream);
      string geoJson = route.ToGeoJson();
      streamWriter.Write(geoJson);
      streamWriter.Flush();
    }

    public static void SaveAsGpx(this Route route, Stream stream)
    {
      RouteGpx.Save(stream, route);
    }

    public static string ToGeoJson(this Route route)
    {
      return GeoJsonConverter.ToGeoJson(route.ToFeatureCollection());
    }

    public static LineString ToLineString(this Route route)
    {
      List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>();
      for (int index = 0; index < route.Segments.Count; ++index)
        geoCoordinateList.Add(new GeoCoordinate((double) route.Segments[index].Latitude, (double) route.Segments[index].Longitude));
      return new LineString((IEnumerable<GeoCoordinate>) geoCoordinateList);
    }

    public static FeatureCollection ToFeatureCollection(this Route route)
    {
      FeatureCollection featureCollection = new FeatureCollection();
      if (route.Segments == null)
        return featureCollection;
      for (int index = 0; index < route.Segments.Count; ++index)
      {
        if (index > 0)
        {
          LineString lineString = new LineString(new GeoCoordinate[2]
          {
            new GeoCoordinate((double) route.Segments[index - 1].Latitude, (double) route.Segments[index - 1].Longitude),
            new GeoCoordinate((double) route.Segments[index].Latitude, (double) route.Segments[index].Longitude)
          });
          RouteTags[] tags = route.Segments[index].Tags;
          SimpleGeometryAttributeCollection attributeCollection = new SimpleGeometryAttributeCollection();
          if (tags != null)
          {
            foreach (RouteTags routeTags in tags)
              attributeCollection.Add(routeTags.Key, (object) routeTags.Value);
          }
          attributeCollection.Add("time", (object) route.Segments[index].Time);
          attributeCollection.Add("distance", (object) route.Segments[index].Distance);
          attributeCollection.Add("profile", (object) route.Segments[index].Profile);
          featureCollection.Add(new Feature((Geometry) lineString, (GeometryAttributeCollection) attributeCollection));
        }
        if (route.Segments[index].Points != null)
        {
          foreach (RouteStop point1 in route.Segments[index].Points)
          {
            RouteTags[] tags = point1.Tags;
            SimpleGeometryAttributeCollection attributeCollection = new SimpleGeometryAttributeCollection();
            if (tags != null)
            {
              foreach (RouteTags routeTags in tags)
                attributeCollection.Add(routeTags.Key, (object) routeTags.Value);
            }
            Point point2 = new Point(new GeoCoordinate((double) point1.Latitude, (double) point1.Longitude));
            featureCollection.Add(new Feature((Geometry) point2, (GeometryAttributeCollection) attributeCollection));
          }
        }
      }
      return featureCollection;
    }

    public static FeatureCollection ToFeatureCollection(this Route route, Func<RouteSegment, RouteSegment, RouteSegment> aggregator)
    {
      RouteSegmentAggregator segmentAggregator = new RouteSegmentAggregator(route, aggregator);
      segmentAggregator.Run();
      if (segmentAggregator.HasRun && segmentAggregator.HasSucceeded)
        return segmentAggregator.Features;
      return new FeatureCollection();
    }

    public static Route Concatenate(this Route route1, Route route2)
    {
      return route1.Concatenate(route2, true);
    }

    public static Route Concatenate(this Route route1, Route route2, bool clone)
    {
      if (route1 == null)
        return route2;
      if (route2 == null)
        return route1;
      if (route1.Segments.Count == 0)
        return route2;
      if (route2.Segments.Count == 0)
        return route1;
      RouteSegment segment1 = route1.Segments[route1.Segments.Count - 1];
      double time = segment1.Time;
      double distance = segment1.Distance;
      RouteSegment segment2 = route2.Segments[0];
      if ((double) System.Math.Abs(segment1.Latitude - segment2.Latitude) >= 0.001 || (double) System.Math.Abs(segment1.Longitude - segment2.Longitude) >= 0.001)
        throw new ArgumentOutOfRangeException("Contatenation of routes can only be done when the end point of the first route equals the start of the second.");
      Route route = new Route();
      List<RouteSegment> routeSegmentList = new List<RouteSegment>();
      for (int index = 0; index < route1.Segments.Count - 1; ++index)
      {
        if (clone)
          routeSegmentList.Add(route1.Segments[index].Clone() as RouteSegment);
        else
          routeSegmentList.Add(route1.Segments[index]);
      }
      RouteSegment routeSegment = route1.Segments[route1.Segments.Count - 1].Clone() as RouteSegment;
      if (route2.Segments[0].Points != null && route2.Segments[0].Points.Length != 0)
      {
        List<RouteStop> routeStopList = new List<RouteStop>();
        if (routeSegment.Points != null)
          routeStopList.AddRange((IEnumerable<RouteStop>) routeSegment.Points);
        for (int index1 = 0; index1 < route2.Segments[0].Points.Length; ++index1)
        {
          bool flag = false;
          for (int index2 = 0; index2 < routeStopList.Count; ++index2)
          {
            if (routeStopList[index2].RepresentsSame(route2.Segments[0].Points[index1]))
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            routeStopList.Add(route2.Segments[0].Points[index1]);
        }
        routeSegment.Points = routeStopList.ToArray();
      }
      routeSegmentList.Add(routeSegment);
      for (int index = 1; index < route2.Segments.Count; ++index)
      {
        if (clone)
          routeSegmentList.Add(route2.Segments[index].Clone() as RouteSegment);
        else
          routeSegmentList.Add(route2.Segments[index]);
        routeSegmentList[routeSegmentList.Count - 1].Distance = routeSegmentList[routeSegmentList.Count - 1].Distance + distance;
        routeSegmentList[routeSegmentList.Count - 1].Time = routeSegmentList[routeSegmentList.Count - 1].Time + time;
      }
      route.Segments = routeSegmentList;
      List<RouteTags> routeTagsList = new List<RouteTags>((route1.Tags == null ? 0 : route1.Tags.Count) + (route2.Tags == null ? 0 : route2.Tags.Count));
      if (route1.Tags != null)
        routeTagsList.AddRange((IEnumerable<RouteTags>) route1.Tags);
      if (route2.Tags != null)
        routeTagsList.AddRange((IEnumerable<RouteTags>) route2.Tags);
      route.Tags = routeTagsList;
      if (route.Segments != null && route.Segments.Count > 0)
      {
        route.TotalDistance = route.Segments[route.Segments.Count - 1].Distance;
        route.TotalTime = route.Segments[route.Segments.Count - 1].Time;
      }
      return route;
    }

    public static GeoCoordinateBox GetBox(this Route route)
    {
      return new GeoCoordinateBox(route.GetPoints().ToArray());
    }

    public static List<GeoCoordinate> GetPoints(this Route route)
    {
      List<GeoCoordinate> geoCoordinateList = new List<GeoCoordinate>(route.Segments.Count);
      for (int index = 0; index < route.Segments.Count; ++index)
        geoCoordinateList.Add(new GeoCoordinate((double) route.Segments[index].Latitude, (double) route.Segments[index].Longitude));
      return geoCoordinateList;
    }

    public static GeoCoordinate PositionAfter(this Route route, Meter m)
    {
      double num1 = 0.0;
      List<GeoCoordinate> points = route.GetPoints();
      for (int index = 0; index < points.Count - 1; ++index)
      {
        double num2 = points[index].DistanceReal(points[index + 1]).Value;
        if (num1 + num2 >= m.Value)
        {
          double num3 = m.Value - num1;
          VectorF2D vectorF2D = ((PointF2D) points[index + 1] - (PointF2D) points[index]) * (num3 / num2);
          PointF2D pointF2D = (PointF2D) points[index] + vectorF2D;
          return new GeoCoordinate(pointF2D[1], pointF2D[0]);
        }
        num1 += num2;
      }
      return (GeoCoordinate) null;
    }

    public static bool ProjectOn(this Route route, GeoCoordinate coordinates, out GeoCoordinate projectedCoordinates)
    {
      int entryIndex;
      Meter distanceFromStart;
      Second timeFromStart;
      return route.ProjectOn(coordinates, out projectedCoordinates, out entryIndex, out distanceFromStart, out timeFromStart);
    }

    public static bool ProjectOn(this Route route, GeoCoordinate coordinates, out GeoCoordinate projectedCoordinates, out Meter distanceToProjected, out Second timeFromStart)
    {
      int entryIndex;
      return route.ProjectOn(coordinates, out projectedCoordinates, out entryIndex, out distanceToProjected, out timeFromStart);
    }

    public static bool ProjectOn(this Route route, GeoCoordinate coordinates, out Meter distanceFromStart)
    {
      GeoCoordinate projectedCoordinates;
      int entryIndex;
      Second timeFromStart;
      return route.ProjectOn(coordinates, out projectedCoordinates, out entryIndex, out distanceFromStart, out timeFromStart);
    }

    public static bool ProjectOn(this Route route, GeoCoordinate coordinates, out GeoCoordinate projectedCoordinates, out int entryIndex, out Meter distanceFromStart, out Second timeFromStart)
    {
      double num1 = double.MaxValue;
      distanceFromStart = (Meter) 0.0;
      timeFromStart = (Second) 0.0;
      double num2 = 0.0;
      projectedCoordinates = (GeoCoordinate) null;
      entryIndex = -1;
      List<GeoCoordinate> points = route.GetPoints();
      for (int index = 0; index < points.Count - 1; ++index)
      {
        PointF2D pointF2D = new GeoCoordinateLine(points[index], points[index + 1], true, true).ProjectOn((PointF2D) coordinates);
        if (pointF2D != (PointF2D) null)
        {
          GeoCoordinate point = new GeoCoordinate(pointF2D[1], pointF2D[0]);
          double num3 = coordinates.Distance(point);
          if (num3 < num1)
          {
            projectedCoordinates = point;
            entryIndex = index;
            num1 = num3;
            double num4 = point.DistanceReal(points[index]).Value;
            distanceFromStart = (Meter) (num2 + num4);
          }
        }
        GeoCoordinate point1 = points[index];
        double num5 = coordinates.Distance(point1);
        if (num5 < num1)
        {
          projectedCoordinates = point1;
          entryIndex = index;
          num1 = num5;
          distanceFromStart = (Meter) num2;
        }
        num2 += points[index].DistanceReal(points[index + 1]).Value;
      }
      GeoCoordinate point2 = points[points.Count - 1];
      if (coordinates.Distance(point2) < num1)
      {
        projectedCoordinates = point2;
        entryIndex = points.Count - 1;
        distanceFromStart = (Meter) num2;
      }
      return true;
    }

    public static void SetStop(this RouteSegment segment, ICoordinate coordinate)
    {
      segment.Points = new RouteStop[1]
      {
        new RouteStop()
        {
          Latitude = coordinate.Latitude,
          Longitude = coordinate.Longitude,
          Metrics = (RouteMetric[]) null,
          Tags = (RouteTags[]) null
        }
      };
    }

    public static void SetStop(this RouteSegment segment, ICoordinate coordinate, TagsCollectionBase tags)
    {
      segment.Points = new RouteStop[1]
      {
        new RouteStop()
        {
          Latitude = coordinate.Latitude,
          Longitude = coordinate.Longitude,
          Metrics = (RouteMetric[]) null,
          Tags = tags.ConvertFrom()
        }
      };
    }

    public static void SetStop(this RouteSegment segment, ICoordinate[] coordinates, TagsCollectionBase[] tags)
    {
      if (coordinates.Length != tags.Length)
        throw new ArgumentException("Coordinates and tags arrays must have the same dimensions.");
      segment.Points = new RouteStop[coordinates.Length];
      for (int index = 0; index < coordinates.Length; ++index)
        segment.Points[index] = new RouteStop()
        {
          Latitude = coordinates[index].Latitude,
          Longitude = coordinates[index].Longitude,
          Metrics = (RouteMetric[]) null,
          Tags = tags[index] == null ? (RouteTags[]) null : tags[index].ConvertFrom()
        };
    }

    public static void SetDistanceAndTime(this RouteSegment segment, RouteSegment previous, Speed speed)
    {
      double num = GeoCoordinate.DistanceEstimateInMeter((ICoordinate) new GeoCoordinateSimple()
      {
        Latitude = previous.Latitude,
        Longitude = previous.Longitude
      }, (ICoordinate) new GeoCoordinateSimple()
      {
        Latitude = segment.Latitude,
        Longitude = segment.Longitude
      });
      segment.Distance = previous.Distance + num;
      segment.Time = previous.Time + num / (double) speed.Value;
    }

    public static void Set(this RouteSegment segment, RouteSegment previous, Profile profile, TagsCollectionBase tags, Speed speed)
    {
      segment.SetDistanceAndTime(previous, speed);
      segment.Tags = tags.ConvertFrom();
      segment.Profile = profile.Name;
    }

    public static void SetSideStreets(this RouteSegment segment, RouterDb routerDb, uint vertex, uint previousEdge, uint nextVertex)
    {
      List<RouteSegmentBranch> routeSegmentBranchList = new List<RouteSegmentBranch>();
      RoutingNetwork.EdgeEnumerator edgeEnumerator = routerDb.Network.GetEdgeEnumerator(vertex);
      while (edgeEnumerator.MoveNext())
      {
        if ((int) edgeEnumerator.Id != (int) previousEdge && (int) edgeEnumerator.To != (int) nextVertex)
        {
          RoutingEdge current = edgeEnumerator.Current;
          RouterDb db = routerDb;
          EdgeData data = current.Data;
          int profile = (int) data.Profile;
          data = current.Data;
          int metaId = (int) data.MetaId;
          TagsCollectionBase profileAndMeta = db.GetProfileAndMeta((uint) profile, (uint) metaId);
          ICoordinate firstPoint = routerDb.Network.GetFirstPoint(current, edgeEnumerator.From);
          routeSegmentBranchList.Add(new RouteSegmentBranch()
          {
            Latitude = firstPoint.Latitude,
            Longitude = firstPoint.Longitude,
            Tags = profileAndMeta.ConvertFrom()
          });
        }
      }
      if (routeSegmentBranchList.Count <= 0)
        return;
      segment.SideStreets = routeSegmentBranchList.ToArray();
    }

    public static RelativeDirection RelativeDirectionAt(this Route route, int i)
    {
      if (i < 0 || i >= route.Segments.Count)
        throw new ArgumentOutOfRangeException("i");
      if (i == 0 || i == route.Segments.Count - 1)
        return (RelativeDirection) null;
      return RelativeDirectionCalculator.Calculate(new GeoCoordinate((double) route.Segments[i - 1].Latitude, (double) route.Segments[i - 1].Longitude), new GeoCoordinate((double) route.Segments[i].Latitude, (double) route.Segments[i].Longitude), new GeoCoordinate((double) route.Segments[i + 1].Latitude, (double) route.Segments[i + 1].Longitude));
    }

    public static DirectionEnum DirectionToNext(this Route route, int i)
    {
      if (i < 0 || i >= route.Segments.Count - 1)
        throw new ArgumentOutOfRangeException("i");
      return DirectionCalculator.Calculate(new GeoCoordinate((double) route.Segments[i].Latitude, (double) route.Segments[i].Longitude), new GeoCoordinate((double) route.Segments[i + 1].Latitude, (double) route.Segments[i + 1].Longitude));
    }
  }
}
