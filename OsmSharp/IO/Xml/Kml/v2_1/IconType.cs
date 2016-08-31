using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  public class IconType : LinkType
  {
    private int xField;
    private int yField;
    private int wField;
    private int hField;

    public int x
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

    public int y
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

    public int w
    {
      get
      {
        return this.wField;
      }
      set
      {
        this.wField = value;
      }
    }

    public int h
    {
      get
      {
        return this.hField;
      }
      set
      {
        this.hField = value;
      }
    }
  }
}
