using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class Point
  {
    private altitudeMode altitudeModeField;
    private bool altitudeModeFieldSpecified;
    private bool extrudeField;
    private bool extrudeFieldSpecified;
    private string coordinatesField;
    private string idField;

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
