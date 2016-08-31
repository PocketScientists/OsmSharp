using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  [XmlRoot(IsNullable = false, Namespace = "")]
  public class changesets
  {
    private long maximum_elementsField;
    private bool maximum_elementsFieldSpecified;

    [XmlAttribute]
    public long maximum_elements
    {
      get
      {
        return this.maximum_elementsField;
      }
      set
      {
        this.maximum_elementsField = value;
      }
    }

    [XmlIgnore]
    public bool maximum_elementsSpecified
    {
      get
      {
        return this.maximum_elementsFieldSpecified;
      }
      set
      {
        this.maximum_elementsFieldSpecified = value;
      }
    }
  }
}
