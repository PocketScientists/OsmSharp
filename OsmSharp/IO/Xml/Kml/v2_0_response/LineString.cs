using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class LineString
  {
    private bool extrudeField;
    private bool extrudeFieldSpecified;
    private bool tessellateField;
    private bool tessellateFieldSpecified;
    private altitudeMode altitudeModeField;
    private bool altitudeModeFieldSpecified;
    private string coordinatesField;
    private string idField;

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

    [XmlIgnore]
    public bool extrudeSpecified
    {
      get
      {
        return this.extrudeFieldSpecified;
      }
      set
      {
        this.extrudeFieldSpecified = value;
      }
    }

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

    [XmlIgnore]
    public bool tessellateSpecified
    {
      get
      {
        return this.tessellateFieldSpecified;
      }
      set
      {
        this.tessellateFieldSpecified = value;
      }
    }

    public altitudeMode altitudeMode
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

    [XmlIgnore]
    public bool altitudeModeSpecified
    {
      get
      {
        return this.altitudeModeFieldSpecified;
      }
      set
      {
        this.altitudeModeFieldSpecified = value;
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
