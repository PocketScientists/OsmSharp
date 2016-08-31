using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("Location", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class LocationType : ObjectType
  {
    private double longitudeField;
    private double latitudeField;
    private double altitudeField;

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

    public LocationType()
    {
      this.longitudeField = 0.0;
      this.latitudeField = 0.0;
      this.altitudeField = 0.0;
    }
  }
}
