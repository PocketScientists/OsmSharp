using OsmSharp.Collections;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using System;

namespace OsmSharp.Osm
{
  public abstract class CompleteOsmBase : IEquatable<CompleteOsmBase>
  {
    private readonly long _id;

    public string User { get; set; }

    public long? UserId { get; set; }

    public long Id
    {
      get
      {
        return this._id;
      }
    }

    public abstract GeoCoordinateBox BoundingBox { get; }

    public abstract CompleteOsmType Type { get; }

    public DateTime? TimeStamp { get; set; }

    public long? Version { get; set; }

    public TagsCollectionBase Tags { get; set; }

    internal CompleteOsmBase(long id)
      : this((ObjectTable<string>) null, id)
    {
    }

    internal CompleteOsmBase(ObjectTable<string> stringTable, long id)
    {
      this._id = id;
      if (stringTable != null)
        this.Tags = (TagsCollectionBase) new StringTableTagsCollection(stringTable);
      else
        this.Tags = (TagsCollectionBase) new TagsCollection();
    }

    public static bool operator ==(CompleteOsmBase a, CompleteOsmBase b)
    {
      if ((object) a == (object) b)
        return true;
      if ((object) a == null || (object) b == null)
        return false;
      return a.Equals(b);
    }

    public static bool operator !=(CompleteOsmBase a, CompleteOsmBase b)
    {
      return !(a == b);
    }

    public bool Equals(CompleteOsmBase other)
    {
      return !(other == (CompleteOsmBase) null) && other._id == this.Id && other.Type == this.Type;
    }

    public override int GetHashCode()
    {
      return this.Type.GetHashCode() ^ this.Id.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      if ((object) (obj as CompleteOsmBase) != null)
        return this.Equals(obj as CompleteOsmBase);
      return false;
    }

    public override string ToString()
    {
      return string.Format("{0}:{1}", new object[2]
      {
        (object) this.Type.ToString(),
        (object) this.Id
      });
    }
  }
}
