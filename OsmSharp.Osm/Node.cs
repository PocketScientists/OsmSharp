using OsmSharp.Collections.Tags;
using OsmSharp.Geo.Features;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Geo.Interpreter;

namespace OsmSharp.Osm
{
  public class Node : OsmGeo, ICompleteOsmGeo
  {
    public static FeatureInterpreter FeatureInterperter = (FeatureInterpreter) new SimpleFeatureInterpreter();
    private FeatureCollection _features;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public GeoCoordinateBox BoundingBox
    {
      get
      {
        return this.Features.Box;
      }
    }

    public GeoCoordinate Coordinate
    {
      get
      {
        double? nullable = this.Latitude;
        double latitude = nullable.Value;
        nullable = this.Longitude;
        double longitude = nullable.Value;
        return new GeoCoordinate(latitude, longitude);
      }
    }

    public FeatureCollection Features
    {
      get
      {
        if (this._features == null)
          this._features = CompleteOsmGeo.FeatureInterperter.Interpret((ICompleteOsmGeo) this);
        return this._features;
      }
    }

    bool ICompleteOsmGeo.Visible
    {
      get
      {
        if (this.Visible.HasValue)
          return this.Visible.Value;
        return false;
      }
      set
      {
        this.Visible = new bool?(value);
      }
    }

    long ICompleteOsmGeo.Id
    {
      get
      {
        return this.Id.Value;
      }
    }

    CompleteOsmType ICompleteOsmGeo.Type
    {
      get
      {
        return CompleteOsmType.Node;
      }
    }

    public Node()
    {
      this.Type = OsmGeoType.Node;
    }

    public override string ToString()
    {
      string str = "{no tags}";
      if (this.Tags != null && this.Tags.Count > 0)
        str = this.Tags.ToString();
      if (!this.Id.HasValue)
        return string.Format("Node[null]{0}", (object) str);
      return string.Format("Node[{0}]{1}", new object[2]
      {
        (object) this.Id.Value,
        (object) str
      });
    }

    public static Node Create(long id, double latitude, double longitude)
    {
      Node node = new Node();
      long? nullable1 = new long?(id);
      node.Id = nullable1;
      double? nullable2 = new double?(latitude);
      node.Latitude = nullable2;
      double? nullable3 = new double?(longitude);
      node.Longitude = nullable3;
      return node;
    }

    public static Node Create(long id, TagsCollectionBase tags, double latitude, double longitude)
    {
      Node node = new Node();
      long? nullable1 = new long?(id);
      node.Id = nullable1;
      double? nullable2 = new double?(latitude);
      node.Latitude = nullable2;
      double? nullable3 = new double?(longitude);
      node.Longitude = nullable3;
      TagsCollectionBase tagsCollectionBase = tags;
      node.Tags = tagsCollectionBase;
      return node;
    }

    public void ResetFeatures()
    {
      this._features = (FeatureCollection) null;
    }

    public OsmGeo ToSimple()
    {
      return (OsmGeo) this;
    }
  }
}
