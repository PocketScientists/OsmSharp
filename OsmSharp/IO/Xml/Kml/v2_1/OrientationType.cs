using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("Orientation", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class OrientationType : ObjectType
  {
    private double headingField;
    private double tiltField;
    private double rollField;

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
    public double roll
    {
      get
      {
        return this.rollField;
      }
      set
      {
        this.rollField = value;
      }
    }

    public OrientationType()
    {
      this.headingField = 0.0;
      this.tiltField = 0.0;
      this.rollField = 0.0;
    }
  }
}
