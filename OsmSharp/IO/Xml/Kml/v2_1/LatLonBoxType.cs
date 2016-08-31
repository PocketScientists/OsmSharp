using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [XmlInclude(typeof (LatLonAltBoxType))]
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("LatLonBox", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class LatLonBoxType : ObjectType
  {
    private double northField;
    private double southField;
    private double eastField;
    private double westField;
    private double rotationField;

    [DefaultValue(180)]
    public double north
    {
      get
      {
        return this.northField;
      }
      set
      {
        this.northField = value;
      }
    }

    [DefaultValue(-180)]
    public double south
    {
      get
      {
        return this.southField;
      }
      set
      {
        this.southField = value;
      }
    }

    [DefaultValue(180)]
    public double east
    {
      get
      {
        return this.eastField;
      }
      set
      {
        this.eastField = value;
      }
    }

    [DefaultValue(-180)]
    public double west
    {
      get
      {
        return this.westField;
      }
      set
      {
        this.westField = value;
      }
    }

    [DefaultValue(0)]
    public double rotation
    {
      get
      {
        return this.rotationField;
      }
      set
      {
        this.rotationField = value;
      }
    }

    public LatLonBoxType()
    {
      this.northField = 180.0;
      this.southField = -180.0;
      this.eastField = 180.0;
      this.westField = -180.0;
      this.rotationField = 0.0;
    }
  }
}
