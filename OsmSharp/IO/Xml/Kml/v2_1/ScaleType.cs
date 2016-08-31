using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("Scale", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class ScaleType : ObjectType
  {
    private double xField;
    private double yField;
    private double zField;

    [DefaultValue(1)]
    public double x
    {
      get
      {
        return this.xField;
      }
      set
      {
        this.xField = value;
      }
    }

    [DefaultValue(1)]
    public double y
    {
      get
      {
        return this.yField;
      }
      set
      {
        this.yField = value;
      }
    }

    [DefaultValue(1)]
    public double z
    {
      get
      {
        return this.zField;
      }
      set
      {
        this.zField = value;
      }
    }

    public ScaleType()
    {
      this.xField = 1.0;
      this.yField = 1.0;
      this.zField = 1.0;
    }
  }
}
