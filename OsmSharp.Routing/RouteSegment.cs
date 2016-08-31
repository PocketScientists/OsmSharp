using OsmSharp.Geo;

namespace OsmSharp.Routing
{
  public class RouteSegment : ICloneable
  {
    public float Latitude { get; set; }

    public float Longitude { get; set; }

    public string Profile { get; set; }

    public RouteTags[] Tags { get; set; }

    public RouteMetric[] Metrics { get; set; }

    public double Distance { get; set; }

    public double Time { get; set; }

    public RouteStop[] Points { get; set; }

    public RouteSegmentBranch[] SideStreets { get; set; }

    public object Clone()
    {
      RouteSegment routeSegment = new RouteSegment();
      routeSegment.Distance = this.Distance;
      routeSegment.Latitude = this.Latitude;
      routeSegment.Longitude = this.Longitude;
      if (this.Metrics != null)
      {
        routeSegment.Metrics = new RouteMetric[this.Metrics.Length];
        for (int index = 0; index < this.Metrics.Length; ++index)
          routeSegment.Metrics[index] = this.Metrics[index].Clone() as RouteMetric;
      }
      if (this.Points != null)
      {
        routeSegment.Points = new RouteStop[this.Points.Length];
        for (int index = 0; index < this.Points.Length; ++index)
          routeSegment.Points[index] = this.Points[index].Clone() as RouteStop;
      }
      if (this.SideStreets != null)
      {
        routeSegment.SideStreets = new RouteSegmentBranch[this.SideStreets.Length];
        for (int index = 0; index < this.SideStreets.Length; ++index)
          routeSegment.SideStreets[index] = this.SideStreets[index].Clone() as RouteSegmentBranch;
      }
      if (this.Tags != null)
      {
        routeSegment.Tags = new RouteTags[this.Tags.Length];
        for (int index = 0; index < this.Tags.Length; ++index)
          routeSegment.Tags[index] = this.Tags[index].Clone() as RouteTags;
      }
      routeSegment.Profile = this.Profile;
      routeSegment.Time = this.Time;
      return (object) routeSegment;
    }

    public static RouteSegment CreateNew(ICoordinate coordinate, OsmSharp.Routing.Profiles.Profile profile)
    {
      return new RouteSegment()
      {
        Latitude = coordinate.Latitude,
        Longitude = coordinate.Longitude,
        Profile = profile.Name
      };
    }

    public static RouteSegment CreateNew(float latitude, float longitude, OsmSharp.Routing.Profiles.Profile profile)
    {
      RouteSegment routeSegment = new RouteSegment();
      routeSegment.Latitude = latitude;
      routeSegment.Longitude = longitude;
      string name = profile.Name;
      routeSegment.Profile = name;
      return routeSegment;
    }

    public override string ToString()
    {
      return string.Format("{2} - @{0}s {1}m", (object) this.Time, (object) this.Distance, (object) this.Profile);
    }
  }
}
