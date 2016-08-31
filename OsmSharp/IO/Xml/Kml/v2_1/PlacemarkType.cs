using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("Placemark", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class PlacemarkType : FeatureType
  {
    private GeometryType item1Field;

    [XmlElement("LineString", typeof (LineStringType))]
    [XmlElement("LinearRing", typeof (LinearRingType))]
    [XmlElement("Model", typeof (ModelType))]
    [XmlElement("MultiGeometry", typeof (MultiGeometryType))]
    [XmlElement("Point", typeof (PointType))]
    [XmlElement("Polygon", typeof (PolygonType))]
    public GeometryType Item1
    {
      get
      {
        return this.item1Field;
      }
      set
      {
        this.item1Field = value;
      }
    }
  }
}
