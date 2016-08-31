using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class LookAt
  {
    private double longitudeField;
    private double latitudeField;
    private double rangeField;
    private double tiltField;
    private double headingField;
    private string timePositionField;
    private string idField;

    public double longitude
    {
      get
      {
        return this.longitudeField;
      }
      set
      {
        this.longitudeField = value;
      }
    }

    public double latitude
    {
      get
      {
        return this.latitudeField;
      }
      set
      {
        this.latitudeField = value;
      }
    }

    public double range
    {
      get
      {
        return this.rangeField;
      }
      set
      {
        this.rangeField = value;
      }
    }

    public double tilt
    {
      get
      {
        return this.tiltField;
      }
      set
      {
        this.tiltField = value;
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

    public string timePosition
    {
      get
      {
        return this.timePositionField;
      }
      set
      {
        this.timePositionField = value;
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
