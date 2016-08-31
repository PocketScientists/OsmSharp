using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("GroundOverlay", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class GroundOverlayType : OverlayType
  {
    private double altitudeField;
    private altitudeModeEnum altitudeModeField;
    private LatLonBoxType latLonBoxField;

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

    public LatLonBoxType LatLonBox
    {
      get
      {
        return this.latLonBoxField;
      }
      set
      {
        this.latLonBoxField = value;
      }
    }

    public GroundOverlayType()
    {
      this.altitudeField = 0.0;
      this.altitudeModeField = altitudeModeEnum.clampToGround;
    }
  }
}
