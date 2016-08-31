using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class size
  {
    private Decimal xField;
    private Decimal yField;
    private sizeXunits xunitsField;
    private sizeYunits yunitsField;
    private string idField;

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
    public sizeXunits xunits
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
    public sizeYunits yunits
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
