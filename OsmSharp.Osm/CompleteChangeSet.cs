using OsmSharp.Math.Geo;
using System.Collections.Generic;

namespace OsmSharp.Osm
{
  public class CompleteChangeSet : CompleteOsmBase
  {
    private readonly IList<CompleteChange> _changes;

    public IList<CompleteChange> Changes
    {
      get
      {
        return this._changes;
      }
    }

    public IList<CompleteOsmGeo> Objects
    {
      get
      {
        List<CompleteOsmGeo> completeOsmGeoList = new List<CompleteOsmGeo>();
        foreach (CompleteChange change in (IEnumerable<CompleteChange>) this.Changes)
          completeOsmGeoList.Add(change.Object);
        return (IList<CompleteOsmGeo>) completeOsmGeoList;
      }
    }

    public override GeoCoordinateBox BoundingBox
    {
      get
      {
        if (this.Objects.Count <= 0)
          return (GeoCoordinateBox) null;
        GeoCoordinateBox boundingBox = this.Objects[0].BoundingBox;
        for (int index = 1; index < this.Objects.Count; ++index)
          boundingBox += this.Objects[index].BoundingBox;
        return boundingBox;
      }
    }

    public override CompleteOsmType Type
    {
      get
      {
        return CompleteOsmType.ChangeSet;
      }
    }

    internal CompleteChangeSet(long id)
      : base(id)
    {
      this._changes = (IList<CompleteChange>) new List<CompleteChange>();
    }
  }
}
