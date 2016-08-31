using OsmSharp.Collections.Tags;
using System;

namespace OsmSharp.Osm
{
  public abstract class OsmGeo
  {
    public long? Id { get; set; }

    public OsmGeoType Type { get; protected set; }

    public TagsCollectionBase Tags { get; set; }

    public long? ChangeSetId { get; set; }

    public bool? Visible { get; set; }

    public DateTime? TimeStamp { get; set; }

    public ulong? Version { get; set; }

    public long? UserId { get; set; }

    public string UserName { get; set; }
  }
}
