using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  [XmlRoot(IsNullable = false, Namespace = "")]
  public class api
  {
    private version versionField;
    private area areaField;
    private tracepoints tracepointsField;
    private waynodes waynodesField;
    private changesets changesetsField;
    private timeout timeoutField;

    [XmlElement("timeout")]
    public timeout timeout
    {
      get
      {
        return this.timeoutField;
      }
      set
      {
        this.timeoutField = value;
      }
    }

    [XmlElement("changesets")]
    public changesets changesets
    {
      get
      {
        return this.changesetsField;
      }
      set
      {
        this.changesetsField = value;
      }
    }

    [XmlElement("waynodes")]
    public waynodes waynodes
    {
      get
      {
        return this.waynodesField;
      }
      set
      {
        this.waynodesField = value;
      }
    }

    [XmlElement("tracepoints")]
    public tracepoints tracepoints
    {
      get
      {
        return this.tracepointsField;
      }
      set
      {
        this.tracepointsField = value;
      }
    }

    [XmlElement("version")]
    public version version
    {
      get
      {
        return this.versionField;
      }
      set
      {
        this.versionField = value;
      }
    }

    [XmlElement("area")]
    public area area
    {
      get
      {
        return this.areaField;
      }
      set
      {
        this.areaField = value;
      }
    }

    public status status { get; set; }
  }
}
