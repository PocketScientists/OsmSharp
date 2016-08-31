using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class PolyStyle
  {
    private byte[] colorField;
    private colorMode colorModeField;
    private bool colorModeFieldSpecified;
    private bool fillField;
    private bool fillFieldSpecified;
    private bool outlineField;
    private bool outlineFieldSpecified;
    private string idField;

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

    public colorMode colorMode
    {
      get
      {
        return this.colorModeField;
      }
      set
      {
        this.colorModeField = value;
      }
    }

    [XmlIgnore]
    public bool colorModeSpecified
    {
      get
      {
        return this.colorModeFieldSpecified;
      }
      set
      {
        this.colorModeFieldSpecified = value;
      }
    }

    public bool fill
    {
      get
      {
        return this.fillField;
      }
      set
      {
        this.fillField = value;
      }
    }

    [XmlIgnore]
    public bool fillSpecified
    {
      get
      {
        return this.fillFieldSpecified;
      }
      set
      {
        this.fillFieldSpecified = value;
      }
    }

    public bool outline
    {
      get
      {
        return this.outlineField;
      }
      set
      {
        this.outlineField = value;
      }
    }

    [XmlIgnore]
    public bool outlineSpecified
    {
      get
      {
        return this.outlineFieldSpecified;
      }
      set
      {
        this.outlineFieldSpecified = value;
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
