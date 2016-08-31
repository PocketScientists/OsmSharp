using OsmSharp.Collections.Tags;
using System.Collections.Generic;

namespace OsmSharp.Osm
{
  public class Way : OsmGeo
  {
    public List<long> Nodes { get; set; }

    public Way()
    {
      this.Type = OsmGeoType.Way;
    }

    public override string ToString()
    {
      string str = "{no tags}";
      if (this.Tags != null && this.Tags.Count > 0)
        str = this.Tags.ToString();
      if (!this.Id.HasValue)
        return string.Format("Way[null]{0}", (object) str);
      return string.Format("Way[{0}]{1}", new object[2]
      {
        (object) this.Id.Value,
        (object) str
      });
    }

    public static Way Create(long id, params long[] nodes)
    {
      Way way = new Way();
      long? nullable = new long?(id);
      way.Id = nullable;
      List<long> longList = new List<long>((IEnumerable<long>) nodes);
      way.Nodes = longList;
      return way;
    }

    public static Way Create(long id, TagsCollectionBase tags, params long[] nodes)
    {
      Way way = new Way();
      long? nullable = new long?(id);
      way.Id = nullable;
      List<long> longList = new List<long>((IEnumerable<long>) nodes);
      way.Nodes = longList;
      TagsCollectionBase tagsCollectionBase = tags;
      way.Tags = tagsCollectionBase;
      return way;
    }
  }
}
