using OsmSharp.Osm.Streams.Collections;
using System.Collections.Generic;

namespace OsmSharp.Osm.Streams
{
  public static class OsmStreamExtensions
  {
    public static OsmStreamSource ToOsmStreamSource(this IEnumerable<OsmGeo> enumerable)
    {
      return (OsmStreamSource) new OsmEnumerableStreamSource(enumerable);
    }
  }
}
