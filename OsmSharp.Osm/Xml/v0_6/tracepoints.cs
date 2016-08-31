using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  [XmlRoot(IsNullable = false, Namespace = "")]
  public class tracepoints
  {
    private long per_pageField;
    private bool per_pageFieldSpecified;

    [XmlAttribute]
    public long per_page
    {
      get
      {
        return this.per_pageField;
      }
      set
      {
        this.per_pageField = value;
      }
    }

    [XmlIgnore]
    public bool per_pageSpecified
    {
      get
      {
        return this.per_pageFieldSpecified;
      }
      set
      {
        this.per_pageFieldSpecified = value;
      }
    }
  }
}
