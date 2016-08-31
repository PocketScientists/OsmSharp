using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class Pair
  {
    private key keyField;
    private string styleUrlField;
    private string idField;

    public key key
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
