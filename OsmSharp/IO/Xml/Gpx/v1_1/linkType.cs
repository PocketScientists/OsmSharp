using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Gpx.v1_1
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
  public class linkType
  {
    private string textField;
    private string typeField;
    private string hrefField;

    public string text
    {
      get
      {
        return this.textField;
      }
      set
      {
        this.textField = value;
      }
    }

    public string type
    {
      get
      {
        return this.typeField;
      }
      set
      {
        this.typeField = value;
      }
    }

    [XmlAttribute(DataType = "anyURI")]
    public string href
    {
      get
      {
        return this.hrefField;
      }
      set
      {
        this.hrefField = value;
      }
    }
  }
}
