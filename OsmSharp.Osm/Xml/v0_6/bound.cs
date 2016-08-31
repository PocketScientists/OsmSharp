using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  [XmlRoot(IsNullable = false, Namespace = "")]
  public class bound
  {
    private string boxField;
    private bool boxFieldSpecified;
    private string originField;
    private bool originFieldSpecified;

    [XmlAttribute]
    public string box
    {
      get
      {
        return this.boxField;
      }
      set
      {
        this.boxField = value;
      }
    }

    [XmlIgnore]
    public bool boxSpecified
    {
      get
      {
        return this.boxFieldSpecified;
      }
      set
      {
        this.boxFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public string origin
    {
      get
      {
        return this.originField;
      }
      set
      {
        this.originField = value;
      }
    }

    [XmlIgnore]
    public bool originSpecified
    {
      get
      {
        return this.originFieldSpecified;
      }
      set
      {
        this.originFieldSpecified = value;
      }
    }
  }
}
