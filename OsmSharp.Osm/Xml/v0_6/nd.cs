using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  [XmlRoot(IsNullable = false, Namespace = "")]
  public class nd
  {
    private long refField;
    private bool refFieldSpecified;

    [XmlAttribute]
    public long @ref
    {
      get
      {
        return this.refField;
      }
      set
      {
        this.refField = value;
      }
    }

    [XmlIgnore]
    public bool refSpecified
    {
      get
      {
        return this.refFieldSpecified;
      }
      set
      {
        this.refFieldSpecified = value;
      }
    }
  }
}
