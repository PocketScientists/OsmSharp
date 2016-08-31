using System.Collections.Generic;

namespace OsmSharp.Geo.Geometries
{
  public class MultiLineString : GeometryCollectionBase<LineString>
  {
    public MultiLineString()
    {
    }

    public MultiLineString(params LineString[] lineStrings)
      : base((IEnumerable<LineString>) lineStrings)
    {
    }

    public MultiLineString(IEnumerable<LineString> lineStrings)
      : base(lineStrings)
    {
    }
  }
}
