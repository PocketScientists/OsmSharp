using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Features;
using OsmSharp.Geo.Geometries;
using OsmSharp.Math.Geo;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Algorithms.Routes
{
  public class RouteSegmentAggregator : AlgorithmBase
  {
    private readonly Route _route;
    private readonly Func<RouteSegment, RouteSegment, RouteSegment> _aggregate;
    private FeatureCollection _features;

    public FeatureCollection Features
    {
      get
      {
        return this._features;
      }
    }

    public RouteSegmentAggregator(Route route, Func<RouteSegment, RouteSegment, RouteSegment> aggregate)
    {
      if (route == null)
        throw new ArgumentNullException("route");
      if (aggregate == null)
        throw new ArgumentNullException("aggregate");
      this._route = route;
      this._aggregate = aggregate;
    }

    protected override void DoRun()
    {
      this._features = new FeatureCollection();
      if (this._route.Segments == null || this._route.Segments.Count == 0)
        return;
      RouteSegment segment = (RouteSegment) null;
      List<GeoCoordinate> shape = new List<GeoCoordinate>();
      shape.Add(new GeoCoordinate((double) this._route.Segments[0].Latitude, (double) this._route.Segments[0].Longitude));
      this.AddPoints(this._route.Segments[0]);
      for (int index = 1; index < this._route.Segments.Count; ++index)
      {
        if (segment == null)
        {
          segment = this._route.Segments[index];
        }
        else
        {
          RouteSegment routeSegment = this._aggregate(segment, this._route.Segments[index]);
          if (routeSegment == null)
          {
            this.AddSegment(segment, shape);
            shape.Clear();
            shape.Add(new GeoCoordinate((double) segment.Latitude, (double) segment.Longitude));
            segment = this._route.Segments[index];
          }
          else
            segment = routeSegment;
        }
        shape.Add(new GeoCoordinate((double) this._route.Segments[index].Latitude, (double) this._route.Segments[index].Longitude));
        this.AddPoints(this._route.Segments[index]);
      }
      if (segment != null)
        this.AddSegment(this._route.Segments[this._route.Segments.Count - 1], shape);
      this.HasSucceeded = true;
    }

    private void AddSegment(RouteSegment segment, List<GeoCoordinate> shape)
    {
      LineString lineString = new LineString((IEnumerable<GeoCoordinate>) shape);
      RouteTags[] tags = segment.Tags;
      SimpleGeometryAttributeCollection attributeCollection = new SimpleGeometryAttributeCollection();
      if (tags != null)
      {
        foreach (RouteTags routeTags in tags)
          attributeCollection.Add(routeTags.Key, (object) routeTags.Value);
      }
      attributeCollection.Add("time", (object) segment.Time);
      attributeCollection.Add("distance", (object) segment.Distance);
      attributeCollection.Add("profile", (object) segment.Profile);
      this._features.Add(new Feature((Geometry) lineString, (GeometryAttributeCollection) attributeCollection));
    }

    private void AddPoints(RouteSegment segment)
    {
      if (segment.Points == null)
        return;
      foreach (RouteStop point in segment.Points)
      {
        RouteTags[] tags = point.Tags;
        SimpleGeometryAttributeCollection attributeCollection = new SimpleGeometryAttributeCollection();
        if (tags != null)
        {
          foreach (RouteTags routeTags in tags)
            attributeCollection.Add(routeTags.Key, (object) routeTags.Value);
        }
        this._features.Add(new Feature((Geometry) new Point(new GeoCoordinate((double) point.Latitude, (double) point.Longitude)), (GeometryAttributeCollection) attributeCollection));
      }
    }
  }
}
