using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class overlayXY
  {
    private string idField;
    private Decimal xField;
    private Decimal yField;
    private overlayXYXunits xunitsField;
    private overlayXYYunits yunitsField;

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

    [XmlAttribute]
    public Decimal x
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
    public Decimal y
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
    public overlayXYXunits xunits
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
    public overlayXYYunits yunits
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
  }
}
