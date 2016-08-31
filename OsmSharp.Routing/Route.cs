using System.Collections.Generic;

namespace OsmSharp.Routing
{
  public class Route
  {
    public List<RouteTags> Tags { get; set; }

    public List<RouteMetric> Metrics { get; set; }

    public List<RouteSegment> Segments { get; set; }

    public double TotalDistance { get; set; }

    public double TotalTime { get; set; }
  }
}
