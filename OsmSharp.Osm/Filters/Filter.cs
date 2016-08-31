using OsmSharp.Osm.Filters.Tags;

namespace OsmSharp.Osm.Filters
{
  public abstract class Filter
  {
    public static Filter operator &(Filter filter1, Filter filter2)
    {
      return (Filter) new FilterCombined(filter1, FilterCombineOperatorEnum.And, filter2);
    }

    public static Filter operator |(Filter filter1, Filter filter2)
    {
      return (Filter) new FilterCombined(filter1, FilterCombineOperatorEnum.Or, filter2);
    }

    public static Filter operator !(Filter filter1)
    {
      return (Filter) new FilterCombined(filter1, FilterCombineOperatorEnum.Not, (Filter) null);
    }

    public bool Evaluate(CompleteOsmGeo obj)
    {
      return this.Evaluate(obj.ToSimple());
    }

    public abstract bool Evaluate(OsmGeo obj);

    public abstract override string ToString();

    public static Filter Type(OsmGeoType type)
    {
      return (Filter) new FilterType(type);
    }

    public static Filter Any()
    {
      return (Filter) new FilterAny();
    }

    public static Filter Match(string tag, string value)
    {
      return (Filter) new FilterTagMatch(tag, value);
    }

    public static Filter MatchAny(string tag, string[] value)
    {
      if (value == null || value.Length == 0)
        return (Filter) null;
      Filter filter = (Filter) new FilterTagMatch(tag, value[0]);
      for (int index = 1; index < value.Length; ++index)
        filter |= (Filter) new FilterTagMatch(tag, value[index]);
      return filter;
    }

    public static Filter Exists(string tag)
    {
      return (Filter) new FilterTagExists(tag);
    }

    public static Filter Exists(string[] tags)
    {
      if (tags == null || tags.Length == 0)
        return (Filter) null;
      Filter filter = (Filter) new FilterTagExists(tags[0]);
      for (int index = 1; index < tags.Length; ++index)
        filter |= (Filter) new FilterTagExists(tags[index]);
      return filter;
    }
  }
}
