using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class Document
  {
    private object[] itemsField;
    private ItemsChoiceType3[] itemsElementNameField;
    private string idField;

    [XmlElement("Document", typeof (Document))]
    [XmlElement("Folder", typeof (Folder))]
    [XmlElement("GroundOverlay", typeof (GroundOverlay))]
    [XmlElement("LookAt", typeof (LookAt))]
    [XmlElement("NetworkLink", typeof (NetworkLink))]
    [XmlElement("Placemark", typeof (Placemark))]
    [XmlElement("Schema", typeof (Schema))]
    [XmlElement("ScreenOverlay", typeof (ScreenOverlay))]
    [XmlElement("Snippet", typeof (Snippet))]
    [XmlElement("Style", typeof (Style))]
    [XmlElement("StyleBlinker", typeof (StyleBlinker))]
    [XmlElement("StyleMap", typeof (StyleMap))]
    [XmlElement("description", typeof (string))]
    [XmlElement("name", typeof (string))]
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
    public ItemsChoiceType3[] ItemsElementName
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
