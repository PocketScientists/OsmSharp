using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("Model", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class ModelType : GeometryType
  {
    private altitudeModeEnum altitudeModeField;
    private LocationType locationField;
    private OrientationType orientationField;
    private ScaleType scaleField;
    private LinkType linkField;

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

    public LocationType Location
    {
      get
      {
        return this.locationField;
      }
      set
      {
        this.locationField = value;
      }
    }

    public OrientationType Orientation
    {
      get
      {
        return this.orientationField;
      }
      set
      {
        this.orientationField = value;
      }
    }

    public ScaleType Scale
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

    public LinkType Link
    {
      get
      {
        return this.linkField;
      }
      set
      {
        this.linkField = value;
      }
    }

    public ModelType()
    {
      this.altitudeModeField = altitudeModeEnum.clampToGround;
    }
  }
}
