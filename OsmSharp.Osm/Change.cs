using System.Collections.Generic;

namespace OsmSharp.Osm
{
  public class Change
  {
    public List<OsmSharp.Osm.OsmGeo> OsmGeo { get; set; }

    public ChangeType Type { get; set; }
  }
}
