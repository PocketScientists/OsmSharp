namespace OsmSharp.Routing
{
  public class RouteSegmentBranch : ICloneable
  {
    public float Latitude { get; set; }

    public float Longitude { get; set; }

    public RouteTags[] Tags { get; set; }

    public object Clone()
    {
      RouteSegmentBranch routeSegmentBranch = new RouteSegmentBranch();
      routeSegmentBranch.Latitude = this.Latitude;
      routeSegmentBranch.Longitude = this.Longitude;
      if (this.Tags != null)
      {
        routeSegmentBranch.Tags = new RouteTags[this.Tags.Length];
        for (int index = 0; index < this.Tags.Length; ++index)
          routeSegmentBranch.Tags[index] = this.Tags[index].Clone() as RouteTags;
      }
      return (object) routeSegmentBranch;
    }
  }
}
