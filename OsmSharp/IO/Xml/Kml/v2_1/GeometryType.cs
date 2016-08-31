using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [XmlInclude(typeof (ModelType))]
  [XmlInclude(typeof (PolygonType))]
  [XmlInclude(typeof (LinearRingType))]
  [XmlInclude(typeof (LineStringType))]
  [XmlInclude(typeof (PointType))]
  [XmlInclude(typeof (MultiGeometryType))]
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  public abstract class GeometryType : ObjectType
  {
  }
}
