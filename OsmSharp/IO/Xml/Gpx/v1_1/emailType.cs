using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Gpx.v1_1
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
  public class emailType
  {
    private string idField;
    private string domainField;

    [XmlAttribute]
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
    public string domain
    {
      get
      {
        return this.domainField;
      }
      set
      {
        this.domainField = value;
      }
    }
  }
}
