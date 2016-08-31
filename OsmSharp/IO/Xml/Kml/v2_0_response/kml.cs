using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class kml
  {
    private object itemField;
    private string idField;

    [XmlElement("Document", typeof (Document))]
    [XmlElement("Folder", typeof (Folder))]
    [XmlElement("GroundOverlay", typeof (GroundOverlay))]
    [XmlElement("LookAt", typeof (LookAt))]
    [XmlElement("NetworkLink", typeof (NetworkLink))]
    [XmlElement("NetworkLinkControl", typeof (NetworkLinkControl))]
    [XmlElement("Placemark", typeof (Placemark))]
    [XmlElement("Response", typeof (Response))]
    [XmlElement("ScreenOverlay", typeof (ScreenOverlay))]
    public object Item
    {
      get
      {
        return this.itemField;
      }
      set
      {
        this.itemField = value;
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
