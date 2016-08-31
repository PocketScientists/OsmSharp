using System.Collections.Generic;

namespace OsmSharp.Geo.Geometries
{
  public class MultiPoint : GeometryCollectionBase<Point>
  {
    public MultiPoint()
    {
    }

    public MultiPoint(params Point[] points)
      : base((IEnumerable<Point>) points)
    {
    }

    public MultiPoint(IEnumerable<Point> points)
      : base(points)
    {
    }
  }
}
