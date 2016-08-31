using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class Folder
  {
    private object[] itemsField;
    private ItemsChoiceType2[] itemsElementNameField;
    private string idField;

    [XmlElement("Document", typeof (Document))]
    [XmlElement("Folder", typeof (Folder))]
    [XmlElement("GroundOverlay", typeof (GroundOverlay))]
    [XmlElement("LookAt", typeof (LookAt))]
    [XmlElement("NetworkLink", typeof (NetworkLink))]
    [XmlElement("Placemark", typeof (Placemark))]
    [XmlElement("ScreenOverlay", typeof (ScreenOverlay))]
    [XmlElement("Snippet", typeof (Snippet))]
    [XmlElement("Style", typeof (Style))]
    [XmlElement("description", typeof (string))]
    [XmlElement("name", typeof (string))]
    [XmlElement("open", typeof (bool))]
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
    public ItemsChoiceType2[] ItemsElementName
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
