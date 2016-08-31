using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [XmlType(IncludeInSchema = false, Namespace = "http://earth.google.com/kml/2.0")]
  public enum ItemsChoiceType1
  {
    LineString,
    LookAt,
    MultiGeometry,
    MultiLineString,
    MultiPoint,
    MultiPolygon,
    Point,
    Polygon,
    Snippet,
    Style,
    TimePeriod,
    address,
    description,
    name,
    styleUrl,
    visibility,
  }
}
