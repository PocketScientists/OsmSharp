using OsmSharp.Collections.Tags;
using OsmSharp.Geo.Features;
using OsmSharp.Math.Geo;

namespace OsmSharp.Osm
{
  public interface ICompleteOsmGeo
  {
    GeoCoordinateBox BoundingBox { get; }

    long? ChangeSetId { get; set; }

    FeatureCollection Features { get; }

    bool Visible { get; set; }

    long Id { get; }

    CompleteOsmType Type { get; }

    TagsCollectionBase Tags { get; set; }

    void ResetFeatures();

    OsmGeo ToSimple();
  }
}
