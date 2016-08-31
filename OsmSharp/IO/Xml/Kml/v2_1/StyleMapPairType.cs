using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  public class StyleMapPairType
  {
    private styleStateEnum keyField;
    private string styleUrlField;

    public styleStateEnum key
    {
      get
      {
        return this.keyField;
      }
      set
      {
        this.keyField = value;
      }
    }

    [XmlElement(DataType = "anyURI")]
    public string styleUrl
    {
      get
      {
        return this.styleUrlField;
      }
      set
      {
        this.styleUrlField = value;
      }
    }
  }
}
