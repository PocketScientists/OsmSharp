using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [XmlType(IncludeInSchema = false, Namespace = "http://earth.google.com/kml/2.0")]
  public enum ItemsChoiceType
  {
    LineString,
    MultiGeometry,
    MultiLineString,
    MultiPoint,
    MultiPolygon,
    Placemark,
    Point,
    Polygon,
    altitudeMode,
    extrude,
    tessellate,
  }
}
