namespace OsmSharp.Routing
{
  public class RouteStop : ICloneable
  {
    public float Latitude { get; set; }

    public float Longitude { get; set; }

    public RouteTags[] Tags { get; set; }

    public RouteMetric[] Metrics { get; set; }

    public object Clone()
    {
      RouteStop routeStop = new RouteStop();
      routeStop.Latitude = this.Latitude;
      routeStop.Longitude = this.Longitude;
      if (this.Metrics != null)
      {
        routeStop.Metrics = new RouteMetric[this.Metrics.Length];
        for (int index = 0; index < this.Metrics.Length; ++index)
          routeStop.Metrics[index] = this.Metrics[index].Clone() as RouteMetric;
      }
      if (this.Tags != null)
      {
        routeStop.Tags = new RouteTags[this.Tags.Length];
        for (int index = 0; index < this.Tags.Length; ++index)
          routeStop.Tags[index] = this.Tags[index].Clone() as RouteTags;
      }
      return (object) routeStop;
    }

    internal bool RepresentsSame(RouteStop routePoint)
    {
      if (routePoint == null || (double) this.Longitude != (double) routePoint.Longitude || (double) this.Latitude != (double) routePoint.Latitude)
        return false;
      if ((routePoint.Tags != null || routePoint.Tags.Length == 0) && (this.Tags != null || this.Tags.Length == 0))
      {
        if (this.Tags.Length != routePoint.Tags.Length)
          return false;
        for (int index = 0; index < this.Tags.Length; ++index)
        {
          if (this.Tags[index].Key != routePoint.Tags[index].Key || this.Tags[index].Value != routePoint.Tags[index].Value)
            return false;
        }
        return true;
      }
      if (this.Tags == null)
        return this.Tags.Length == 0;
      return true;
    }
  }
}
