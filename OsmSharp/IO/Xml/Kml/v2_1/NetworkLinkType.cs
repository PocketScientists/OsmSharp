using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("NetworkLink", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class NetworkLinkType : FeatureType
  {
    private bool refreshVisibilityField;
    private bool flyToViewField;
    private LinkType item1Field;
    private Item1ChoiceType item1ElementNameField;

    [DefaultValue(false)]
    public bool refreshVisibility
    {
      get
      {
        return this.refreshVisibilityField;
      }
      set
      {
        this.refreshVisibilityField = value;
      }
    }

    [DefaultValue(false)]
    public bool flyToView
    {
      get
      {
        return this.flyToViewField;
      }
      set
      {
        this.flyToViewField = value;
      }
    }

    [XmlElement("Link", typeof (LinkType))]
    [XmlElement("Url", typeof (LinkType))]
    [XmlChoiceIdentifier("Item1ElementName")]
    public LinkType Item1
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

    [XmlIgnore]
    public Item1ChoiceType Item1ElementName
    {
      get
      {
        return this.item1ElementNameField;
      }
      set
      {
        this.item1ElementNameField = value;
      }
    }

    public NetworkLinkType()
    {
      this.refreshVisibilityField = false;
      this.flyToViewField = false;
    }
  }
}
