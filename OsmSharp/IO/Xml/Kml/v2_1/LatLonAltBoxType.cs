using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("LatLonAltBox", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class LatLonAltBoxType : LatLonBoxType
  {
    private double minAltitudeField;
    private double maxAltitudeField;
    private altitudeModeEnum altitudeModeField;

    [DefaultValue(0)]
    public double minAltitude
    {
      get
      {
        return this.minAltitudeField;
      }
      set
      {
        this.minAltitudeField = value;
      }
    }

    [DefaultValue(0)]
    public double maxAltitude
    {
      get
      {
        return this.maxAltitudeField;
      }
      set
      {
        this.maxAltitudeField = value;
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

    public LatLonAltBoxType()
    {
      this.minAltitudeField = 0.0;
      this.maxAltitudeField = 0.0;
      this.altitudeModeField = altitudeModeEnum.clampToGround;
    }
  }
}
