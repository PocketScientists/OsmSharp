namespace OsmSharp.Osm.Filters
{
  internal class FilterType : Filter
  {
    private OsmGeoType _type;

    internal FilterType(OsmGeoType type)
    {
      this._type = type;
    }

    public override bool Evaluate(OsmGeo obj)
    {
      return obj.Type == this._type;
    }

    public override string ToString()
    {
      return string.Format("istype:{0}", (object) this._type.ToString());
    }
  }
}
