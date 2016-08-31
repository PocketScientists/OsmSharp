using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("LookAt", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class LookAtType : ObjectType
  {
    private double longitudeField;
    private double latitudeField;
    private double altitudeField;
    private double rangeField;
    private bool rangeFieldSpecified;
    private double tiltField;
    private double headingField;
    private altitudeModeEnum altitudeModeField;

    [DefaultValue(0)]
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

    [DefaultValue(0)]
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

    [DefaultValue(0)]
    public double altitude
    {
      get
      {
        return this.altitudeField;
      }
      set
      {
        this.altitudeField = value;
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

    [XmlIgnore]
    public bool rangeSpecified
    {
      get
      {
        return this.rangeFieldSpecified;
      }
      set
      {
        this.rangeFieldSpecified = value;
      }
    }

    [DefaultValue(0)]
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

    [DefaultValue(0)]
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

    [DefaultValue(altitudeModeEnum.clampToGround)]
    public altitudeModeEnum altitudeMode
    {
      get
      {
        return this.altitudeModeField;
      }
      set
      {
        this.altitudeModeField = value;
      }
    }

    public LookAtType()
    {
      this.longitudeField = 0.0;
      this.latitudeField = 0.0;
      this.altitudeField = 0.0;
      this.tiltField = 0.0;
      this.headingField = 0.0;
      this.altitudeModeField = altitudeModeEnum.clampToGround;
    }
  }
}
