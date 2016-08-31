using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("Document", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class DocumentType : ContainerType
  {
    private FeatureType[] items1Field;

    [XmlElement("Document", typeof (DocumentType))]
    [XmlElement("Folder", typeof (FolderType))]
    [XmlElement("GroundOverlay", typeof (GroundOverlayType))]
    [XmlElement("NetworkLink", typeof (NetworkLinkType))]
    [XmlElement("Placemark", typeof (PlacemarkType))]
    [XmlElement("ScreenOverlay", typeof (ScreenOverlayType))]
    public FeatureType[] Items1
    {
      get
      {
        return this.items1Field;
      }
      set
      {
        this.items1Field = value;
      }
    }
  }
}
