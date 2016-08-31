namespace OsmSharp.Osm.Filters.Tags
{
  internal class FilterTagExists : FilterTag
  {
    private string _tag;

    public FilterTagExists(string tag)
    {
      this._tag = tag;
    }

    public override bool Evaluate(OsmGeo obj)
    {
      return obj.Tags.ContainsKey(this._tag);
    }

    public override string ToString()
    {
      return string.Format("hastag:key={0}", (object) this._tag);
    }
  }
}
