using OsmSharp.Collections.Tags;
using System;

namespace OsmSharp.Routing
{
  public class RouterPoint
  {
    public uint EdgeId { get; private set; }

    public ushort Offset { get; private set; }

    public float Latitude { get; private set; }

    public float Longitude { get; private set; }

    public TagsCollectionBase Tags { get; private set; }

    public RouterPoint(float latitude, float longitude, uint edgeId, ushort offset)
    {
      this.Latitude = latitude;
      this.Longitude = longitude;
      this.EdgeId = edgeId;
      this.Offset = offset;
      this.Tags = (TagsCollectionBase) new TagsCollection();
    }

    public RouterPoint(float latitude, float longitude, uint edgeId, ushort offset, params Tag[] tags)
    {
      this.Latitude = latitude;
      this.Longitude = longitude;
      this.EdgeId = edgeId;
      this.Offset = offset;
      this.Tags = (TagsCollectionBase) new TagsCollection(tags);
    }

    public override string ToString()
    {
      return string.Format("{0}@{1}% [{2},{3}] {4}", (object) this.EdgeId, (object) System.Math.Round((double) this.Offset / (double) ushort.MaxValue * 100.0, 1).ToInvariantString(), (object) this.Latitude.ToInvariantString(), (object) this.Longitude.ToInvariantString(), (object) this.Tags.ToInvariantString());
    }
  }
}
