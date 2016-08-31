using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class Placemark
  {
    private object[] itemsField;
    private ItemsChoiceType1[] itemsElementNameField;
    private string idField;

    [XmlElement("LineString", typeof (LineString))]
    [XmlElement("LookAt", typeof (LookAt))]
    [XmlElement("MultiGeometry", typeof (MultiGeometry))]
    [XmlElement("MultiLineString", typeof (MultiLineString))]
    [XmlElement("MultiPoint", typeof (MultiPoint))]
    [XmlElement("MultiPolygon", typeof (MultiPolygon))]
    [XmlElement("Point", typeof (Point))]
    [XmlElement("Polygon", typeof (Polygon))]
    [XmlElement("Snippet", typeof (Snippet))]
    [XmlElement("Style", typeof (Style))]
    [XmlElement("TimePeriod", typeof (TimePeriod))]
    [XmlElement("address", typeof (string))]
    [XmlElement("description", typeof (string))]
    [XmlElement("name", typeof (string))]
    [XmlElement("styleUrl", typeof (string), DataType = "anyURI")]
    [XmlElement("visibility", typeof (bool))]
    [XmlChoiceIdentifier("ItemsElementName")]
    public object[] Items
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

    [XmlElement("ItemsElementName")]
    [XmlIgnore]
    public ItemsChoiceType1[] ItemsElementName
    {
      get
      {
        return this.itemsElementNameField;
      }
      set
      {
        this.itemsElementNameField = value;
      }
    }

    [XmlAttribute(DataType = "ID")]
    public string id
    {
      get
      {
        return this.idField;
      }
      set
      {
        this.idField = value;
      }
    }
  }
}
