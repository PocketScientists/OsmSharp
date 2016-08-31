using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  public class vec2Type
  {
    private double xField;
    private double yField;
    private unitsEnum xunitsField;
    private unitsEnum yunitsField;

    [XmlAttribute]
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

    [XmlAttribute]
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

    [XmlAttribute]
    [DefaultValue(unitsEnum.fraction)]
    public unitsEnum xunits
    {
      get
      {
        return this.xunitsField;
      }
      set
      {
        this.xunitsField = value;
      }
    }

    [XmlAttribute]
    [DefaultValue(unitsEnum.fraction)]
    public unitsEnum yunits
    {
      get
      {
        return this.yunitsField;
      }
      set
      {
        this.yunitsField = value;
      }
    }

    public vec2Type()
    {
      this.xField = 1.0;
      this.yField = 1.0;
      this.xunitsField = unitsEnum.fraction;
      this.yunitsField = unitsEnum.fraction;
    }
  }
}
