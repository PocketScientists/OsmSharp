using OsmSharp.Collections;
using OsmSharp.Geo.Features;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Geo.Interpreter;

namespace OsmSharp.Osm
{
  public abstract class CompleteOsmGeo : CompleteOsmBase, ICompleteOsmGeo
  {
    public static FeatureInterpreter FeatureInterperter = (FeatureInterpreter) new SimpleFeatureInterpreter();
    private FeatureCollection _features;

    public override GeoCoordinateBox BoundingBox
    {
      get
      {
        return this.Features.Box;
      }
    }

    public long? ChangeSetId { get; set; }

    public bool Visible { get; set; }

    public FeatureCollection Features
    {
      get
      {
        if (this._features == null)
          this._features = CompleteOsmGeo.FeatureInterperter.Interpret((ICompleteOsmGeo) this);
        return this._features;
      }
    }

    internal CompleteOsmGeo(long id)
      : base(id)
    {
      this.Visible = true;
      this.UserId = new long?();
      this.User = (string) null;
    }

    internal CompleteOsmGeo(ObjectTable<string> string_table, long id)
      : base(string_table, id)
    {
      this.Visible = true;
      this.UserId = new long?();
      this.User = (string) null;
    }

    public abstract OsmGeo ToSimple();

    public void ResetFeatures()
    {
      this._features = (FeatureCollection) null;
    }
  }
}
