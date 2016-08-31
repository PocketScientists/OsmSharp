namespace OsmSharp.Osm.Filters.Tags
{
  internal class FilterTagMatch : FilterTag
  {
    private readonly string _key;
    private readonly string _value;

    public FilterTagMatch(string key, string value)
    {
      this._key = key;
      this._value = value;
    }

    public override bool Evaluate(OsmGeo obj)
    {
      string str;
      if (obj.Tags != null && obj.Tags.TryGetValue(this._key, out str))
        return str == this._value;
      return false;
    }

    public override string ToString()
    {
      return string.Format("hastag:key={0} and value={1}", new object[2]
      {
        (object) this._key,
        (object) this._value
      });
    }
  }
}
