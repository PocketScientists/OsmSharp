using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  [XmlRoot(IsNullable = false, Namespace = "")]
  public class member
  {
    private memberType typeField;
    private bool typeFieldSpecified;
    private long refField;
    private bool refFieldSpecified;
    private string roleField;

    [XmlAttribute]
    public memberType type
    {
      get
      {
        return this.typeField;
      }
      set
      {
        this.typeField = value;
      }
    }

    [XmlIgnore]
    public bool typeSpecified
    {
      get
      {
        return this.typeFieldSpecified;
      }
      set
      {
        this.typeFieldSpecified = value;
      }
    }

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

    [XmlAttribute]
    public string role
    {
      get
      {
        return this.roleField;
      }
      set
      {
        this.roleField = value;
      }
    }
  }
}
