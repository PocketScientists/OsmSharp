using OsmSharp.Collections.Tags;
using System.Collections.Generic;

namespace OsmSharp.Routing.Attributes
{
  public static class AttributesIndexExtensions
  {
    public static uint Add(this AttributesIndex index, IEnumerable<Tag> tags)
    {
      return index.Add((TagsCollectionBase) new TagsCollection(tags));
    }

    public static uint Add(this AttributesIndex index, params Tag[] tags)
    {
      return index.Add((TagsCollectionBase) new TagsCollection(tags));
    }
  }
}
