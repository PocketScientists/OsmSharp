using OsmSharp.Geo.Attributes;
using OsmSharp.Geo.Geometries;

namespace OsmSharp.Geo.Features
{
  public class Feature
  {
    public Geometry Geometry { get; private set; }

    public GeometryAttributeCollection Attributes { get; private set; }

    public Feature(Geometry geometry)
    {
      this.Geometry = geometry;
      this.Attributes = (GeometryAttributeCollection) new SimpleGeometryAttributeCollection();
    }

    public Feature(Geometry geometry, GeometryAttributeCollection attributes)
    {
      this.Geometry = geometry;
      this.Attributes = attributes;
    }
  }
}
