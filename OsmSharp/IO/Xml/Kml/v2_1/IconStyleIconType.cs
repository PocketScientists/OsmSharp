using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  public class IconStyleIconType : ObjectType
  {
    private string hrefField;

    [XmlElement(DataType = "anyURI")]
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
