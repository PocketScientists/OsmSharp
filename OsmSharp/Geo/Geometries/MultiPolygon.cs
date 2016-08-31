using System.Collections.Generic;

namespace OsmSharp.Geo.Geometries
{
  public class MultiPolygon : GeometryCollectionBase<Polygon>
  {
    public MultiPolygon()
    {
    }

    public MultiPolygon(params Polygon[] polygons)
      : base((IEnumerable<Polygon>) polygons)
    {
    }

    public MultiPolygon(IEnumerable<Polygon> polygons)
      : base(polygons)
    {
    }
  }
}
