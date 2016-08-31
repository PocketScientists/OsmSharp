using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  [XmlRoot(IsNullable = false, Namespace = "")]
  public class bounds
  {
    private double minlatField;
    private bool minlatFieldSpecified;
    private double minlonField;
    private bool minlonFieldSpecified;
    private double maxlatField;
    private bool maxlatFieldSpecified;
    private double maxlonField;
    private bool maxlonFieldSpecified;

    [XmlAttribute]
    public double minlat
    {
      get
      {
        return this.minlatField;
      }
      set
      {
        this.minlatField = value;
      }
    }

    [XmlIgnore]
    public bool minlatSpecified
    {
      get
      {
        return this.minlatFieldSpecified;
      }
      set
      {
        this.minlatFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public double minlon
    {
      get
      {
        return this.minlonField;
      }
      set
      {
        this.minlonField = value;
      }
    }

    [XmlIgnore]
    public bool minlonSpecified
    {
      get
      {
        return this.minlonFieldSpecified;
      }
      set
      {
        this.minlonFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public double maxlat
    {
      get
      {
        return this.maxlatField;
      }
      set
      {
        this.maxlatField = value;
      }
    }

    [XmlIgnore]
    public bool maxlatSpecified
    {
      get
      {
        return this.maxlatFieldSpecified;
      }
      set
      {
        this.maxlatFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public double maxlon
    {
      get
      {
        return this.maxlonField;
      }
      set
      {
        this.maxlonField = value;
      }
    }

    [XmlIgnore]
    public bool maxlonSpecified
    {
      get
      {
        return this.maxlonFieldSpecified;
      }
      set
      {
        this.maxlonFieldSpecified = value;
      }
    }
  }
}
