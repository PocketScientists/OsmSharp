using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("LineString", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class LineStringType : GeometryType
  {
    private bool extrudeField;
    private bool tessellateField;
    private altitudeModeEnum altitudeModeField;
    private string coordinatesField;

    [DefaultValue(false)]
    public bool extrude
    {
      get
      {
        return this.extrudeField;
      }
      set
      {
        this.extrudeField = value;
      }
    }

    [DefaultValue(false)]
    public bool tessellate
    {
      get
      {
        return this.tessellateField;
      }
      set
      {
        this.tessellateField = value;
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

    public string coordinates
    {
      get
      {
        return this.coordinatesField;
      }
      set
      {
        this.coordinatesField = value;
      }
    }

    public LineStringType()
    {
      this.extrudeField = false;
      this.tessellateField = false;
      this.altitudeModeField = altitudeModeEnum.clampToGround;
    }
  }
}
