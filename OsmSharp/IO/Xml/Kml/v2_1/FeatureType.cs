using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [XmlInclude(typeof (OverlayType))]
  [XmlInclude(typeof (ScreenOverlayType))]
  [XmlInclude(typeof (GroundOverlayType))]
  [XmlInclude(typeof (NetworkLinkType))]
  [XmlInclude(typeof (PlacemarkType))]
  [XmlInclude(typeof (ContainerType))]
  [XmlInclude(typeof (FolderType))]
  [XmlInclude(typeof (DocumentType))]
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  public abstract class FeatureType : ObjectType
  {
    private string nameField;
    private bool visibilityField;
    private bool openField;
    private string addressField;
    private string phoneNumberField;
    private SnippetType snippetField;
    private string descriptionField;
    private LookAtType lookAtField;
    private TimePrimitiveType itemField;
    private string styleUrlField;
    private StyleSelectorType[] itemsField;
    private RegionType regionField;

    public string name
    {
      get
      {
        return this.nameField;
      }
      set
      {
        this.nameField = value;
      }
    }

    [DefaultValue(true)]
    public bool visibility
    {
      get
      {
        return this.visibilityField;
      }
      set
      {
        this.visibilityField = value;
      }
    }

    [DefaultValue(true)]
    public bool open
    {
      get
      {
        return this.openField;
      }
      set
      {
        this.openField = value;
      }
    }

    public string address
    {
      get
      {
        return this.addressField;
      }
      set
      {
        this.addressField = value;
      }
    }

    public string phoneNumber
    {
      get
      {
        return this.phoneNumberField;
      }
      set
      {
        this.phoneNumberField = value;
      }
    }

    public SnippetType Snippet
    {
      get
      {
        return this.snippetField;
      }
      set
      {
        this.snippetField = value;
      }
    }

    public string description
    {
      get
      {
        return this.descriptionField;
      }
      set
      {
        this.descriptionField = value;
      }
    }

    public LookAtType LookAt
    {
      get
      {
        return this.lookAtField;
      }
      set
      {
        this.lookAtField = value;
      }
    }

    [XmlElement("TimeSpan", typeof (TimeSpanType))]
    [XmlElement("TimeStamp", typeof (TimeStampType))]
    public TimePrimitiveType Item
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

    [XmlElement(DataType = "anyURI")]
    public string styleUrl
    {
      get
      {
        return this.styleUrlField;
      }
      set
      {
        this.styleUrlField = value;
      }
    }

    [XmlElement("Style", typeof (StyleType))]
    [XmlElement("StyleMap", typeof (StyleMapType))]
    public StyleSelectorType[] Items
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

    public RegionType Region
    {
      get
      {
        return this.regionField;
      }
      set
      {
        this.regionField = value;
      }
    }

    public FeatureType()
    {
      this.visibilityField = true;
      this.openField = true;
    }
  }
}
