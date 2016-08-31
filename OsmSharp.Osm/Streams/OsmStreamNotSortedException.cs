using System;

namespace OsmSharp.Osm.Streams
{
  public class OsmStreamNotSortedException : Exception
  {
    public OsmStreamNotSortedException(string message)
      : base(message)
    {
    }
  }
}
