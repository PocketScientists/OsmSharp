using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class LabelStyle
  {
    private byte[] colorField;
    private colorMode colorModeField;
    private bool colorModeFieldSpecified;
    private Decimal scaleField;
    private bool scaleFieldSpecified;
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

    public Decimal scale
    {
      get
      {
        return this.scaleField;
      }
      set
      {
        this.scaleField = value;
      }
    }

    [XmlIgnore]
    public bool scaleSpecified
    {
      get
      {
        return this.scaleFieldSpecified;
      }
      set
      {
        this.scaleFieldSpecified = value;
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
