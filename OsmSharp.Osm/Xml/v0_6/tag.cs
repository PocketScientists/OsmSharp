using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  [XmlRoot(IsNullable = false, Namespace = "")]
  public class tag
  {
    private string kField;
    private string vField;

    [XmlAttribute]
    public string k
    {
      get
      {
        return this.kField;
      }
      set
      {
        this.kField = value;
      }
    }

    [XmlAttribute]
    public string v
    {
      get
      {
        return this.vField;
      }
      set
      {
        this.vField = value;
      }
    }
  }
}
