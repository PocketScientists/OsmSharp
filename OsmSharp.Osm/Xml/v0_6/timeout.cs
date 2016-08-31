using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  [XmlRoot(IsNullable = false, Namespace = "")]
  public class timeout
  {
    private long secondsField;
    private bool secondsFieldSpecified;

    [XmlAttribute]
    public long seconds
    {
      get
      {
        return this.secondsField;
      }
      set
      {
        this.secondsField = value;
      }
    }

    [XmlIgnore]
    public bool secondsSpecified
    {
      get
      {
        return this.secondsFieldSpecified;
      }
      set
      {
        this.secondsFieldSpecified = value;
      }
    }
  }
}
