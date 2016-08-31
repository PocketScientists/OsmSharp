using System;
using System.Collections.Generic;

namespace OsmSharp.Osm
{
  public class ChangeSetInfo
  {
    public long? Id { get; set; }

    public DateTime? ClosedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public Dictionary<string, string> Tags { get; set; }

    public bool Open { get; set; }

    public double MinLon { get; set; }

    public double MinLat { get; set; }

    public double MaxLon { get; set; }

    public double MaxLat { get; set; }

    public long UserId { get; set; }

    public string User { get; set; }
  }
}
