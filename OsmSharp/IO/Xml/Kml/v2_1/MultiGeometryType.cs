using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("MultiGeometry", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class MultiGeometryType : GeometryType
  {
    private GeometryType[] itemsField;

    [XmlElement("LineString", typeof (LineStringType))]
    [XmlElement("LinearRing", typeof (LinearRingType))]
    [XmlElement("Model", typeof (ModelType))]
    [XmlElement("MultiGeometry", typeof (MultiGeometryType))]
    [XmlElement("Point", typeof (PointType))]
    [XmlElement("Polygon", typeof (PolygonType))]
    public GeometryType[] Items
    {
      get
      {
        return this.itemsField;
      }
      set
      {
        this.itemsField = value;
      }
    }
  }
}
