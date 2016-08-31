using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class IconStyle
  {
    private byte[] colorField;
    private colorMode colorModeField;
    private bool colorModeFieldSpecified;
    private double headingField;
    private bool headingFieldSpecified;
    private Icon iconField;
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

    public double heading
    {
      get
      {
        return this.headingField;
      }
      set
      {
        this.headingField = value;
      }
    }

    [XmlIgnore]
    public bool headingSpecified
    {
      get
      {
        return this.headingFieldSpecified;
      }
      set
      {
        this.headingFieldSpecified = value;
      }
    }

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
