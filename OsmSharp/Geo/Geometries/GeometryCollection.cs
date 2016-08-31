using System.Collections.Generic;

namespace OsmSharp.Geo.Geometries
{
  public class GeometryCollection : GeometryCollectionBase<Geometry>
  {
    public GeometryCollection()
    {
    }

    public GeometryCollection(params Geometry[] geometries)
      : base((IEnumerable<Geometry>) geometries)
    {
    }

    public GeometryCollection(IEnumerable<Geometry> geometries)
      : base(geometries)
    {
    }
  }
}
