using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class GroundOverlay
  {
    private Icon iconField;
    private LatLonBox latLonBoxField;
    private int drawOrderField;
    private bool drawOrderFieldSpecified;
    private string descriptionField;
    private string nameField;
    private LookAt lookAtField;
    private bool visibilityField;
    private bool visibilityFieldSpecified;
    private int refreshIntervalField;
    private bool refreshIntervalFieldSpecified;
    private byte[] colorField;
    private string idField;

    public Icon Icon
    {
      get
      {
        return this.iconField;
      }
      set
      {
        this.iconField = value;
      }
    }

    public LatLonBox LatLonBox
    {
      get
      {
        return this.latLonBoxField;
      }
      set
      {
        this.latLonBoxField = value;
      }
    }

    public int drawOrder
    {
      get
      {
        return this.drawOrderField;
      }
      set
      {
        this.drawOrderField = value;
      }
    }

    [XmlIgnore]
    public bool drawOrderSpecified
    {
      get
      {
        return this.drawOrderFieldSpecified;
      }
      set
      {
        this.drawOrderFieldSpecified = value;
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

    public LookAt LookAt
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

    [XmlIgnore]
    public bool visibilitySpecified
    {
      get
      {
        return this.visibilityFieldSpecified;
      }
      set
      {
        this.visibilityFieldSpecified = value;
      }
    }

    public int refreshInterval
    {
      get
      {
        return this.refreshIntervalField;
      }
      set
      {
        this.refreshIntervalField = value;
      }
    }

    [XmlIgnore]
    public bool refreshIntervalSpecified
    {
      get
      {
        return this.refreshIntervalFieldSpecified;
      }
      set
      {
        this.refreshIntervalFieldSpecified = value;
      }
    }

    [XmlElement(DataType = "hexBinary")]
    public byte[] color
    {
      get
      {
        return this.colorField;
      }
      set
      {
        this.colorField = value;
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
