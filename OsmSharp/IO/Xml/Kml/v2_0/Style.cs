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
  public class Style
  {
    private Icon iconField;
    private IconStyle iconStyleField;
    private LineStyle lineStyleField;
    private PolyStyle polyStyleField;
    private LabelStyle labelStyleField;
    private BalloonStyle balloonStyleField;
    private byte[] colorField;
    private Decimal scaleField;
    private bool scaleFieldSpecified;
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

    public IconStyle IconStyle
    {
      get
      {
        return this.iconStyleField;
      }
      set
      {
        this.iconStyleField = value;
      }
    }

    public LineStyle LineStyle
    {
      get
      {
        return this.lineStyleField;
      }
      set
      {
        this.lineStyleField = value;
      }
    }

    public PolyStyle PolyStyle
    {
      get
      {
        return this.polyStyleField;
      }
      set
      {
        this.polyStyleField = value;
      }
    }

    public LabelStyle LabelStyle
    {
      get
      {
        return this.labelStyleField;
      }
      set
      {
        this.labelStyleField = value;
      }
    }

    public BalloonStyle BalloonStyle
    {
      get
      {
        return this.balloonStyleField;
      }
      set
      {
        this.balloonStyleField = value;
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
