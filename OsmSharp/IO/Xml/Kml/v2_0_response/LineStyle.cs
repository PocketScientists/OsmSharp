using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class LineStyle
  {
    private byte[] colorField;
    private colorMode colorModeField;
    private bool colorModeFieldSpecified;
    private int widthField;
    private bool widthFieldSpecified;
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

    public int width
    {
      get
      {
        return this.widthField;
      }
      set
      {
        this.widthField = value;
      }
    }

    [XmlIgnore]
    public bool widthSpecified
    {
      get
      {
        return this.widthFieldSpecified;
      }
      set
      {
        this.widthFieldSpecified = value;
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
