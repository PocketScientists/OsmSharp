namespace OsmSharp.Osm.API
{
  public class APICapabilities
  {
    public double VersionMinimum { get; internal set; }

    public double VersionMaximum { get; internal set; }

    public double AreaMaximum { get; internal set; }

    public long TracePointsPerPage { get; internal set; }

    public long WayNodesMaximum { get; internal set; }

    public long ChangeSetsMaximumElement { get; internal set; }

    public long TimeoutSeconds { get; internal set; }
  }
}
