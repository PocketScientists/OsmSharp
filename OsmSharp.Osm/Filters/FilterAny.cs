namespace OsmSharp.Osm.Filters
{
  internal class FilterAny : Filter
  {
    public override bool Evaluate(OsmGeo obj)
    {
      return true;
    }

    public override string ToString()
    {
      return string.Format("*");
    }
  }
}
