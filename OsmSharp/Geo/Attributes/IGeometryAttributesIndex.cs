namespace OsmSharp.Geo.Attributes
{
  public interface IGeometryAttributesIndex
  {
    GeometryAttributeCollection Get(uint attributesId);

    uint Add(GeometryAttributeCollection attributes);
  }
}
