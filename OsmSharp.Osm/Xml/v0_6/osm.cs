using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  [XmlRoot(IsNullable = false, Namespace = "")]
  public class osm
  {
    private api apiField;
    private bounds boundsField;
    private bound boundField;
    private OsmSharp.Osm.Xml.v0_6.node[] nodeField;
    private OsmSharp.Osm.Xml.v0_6.way[] wayField;
    private OsmSharp.Osm.Xml.v0_6.relation[] relationField;
    private OsmSharp.Osm.Xml.v0_6.changeset[] changesetField;
    private double versionField;
    private bool versionFieldSpecified;
    private string generatorField;

    [XmlElement("api")]
    public api api
    {
      get
      {
        return this.apiField;
      }
      set
      {
        this.apiField = value;
      }
    }

    [XmlElement("bounds")]
    public bounds bounds
    {
      get
      {
        return this.boundsField;
      }
      set
      {
        this.boundsField = value;
      }
    }

    [XmlElement("bound")]
    public bound bound
    {
      get
      {
        return this.boundField;
      }
      set
      {
        this.boundField = value;
      }
    }

    [XmlElement("node")]
    public OsmSharp.Osm.Xml.v0_6.node[] node
    {
      get
      {
        return this.nodeField;
      }
      set
      {
        this.nodeField = value;
      }
    }

    [XmlElement("way")]
    public OsmSharp.Osm.Xml.v0_6.way[] way
    {
      get
      {
        return this.wayField;
      }
      set
      {
        this.wayField = value;
      }
    }

    [XmlElement("relation")]
    public OsmSharp.Osm.Xml.v0_6.relation[] relation
    {
      get
      {
        return this.relationField;
      }
      set
      {
        this.relationField = value;
      }
    }

    [XmlElement("changeset")]
    public OsmSharp.Osm.Xml.v0_6.changeset[] changeset
    {
      get
      {
        return this.changesetField;
      }
      set
      {
        this.changesetField = value;
      }
    }

    [XmlAttribute]
    public double version
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

    [XmlIgnore]
    public bool versionSpecified
    {
      get
      {
        return this.versionFieldSpecified;
      }
      set
      {
        this.versionFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public string generator
    {
      get
      {
        return this.generatorField;
      }
      set
      {
        this.generatorField = value;
      }
    }

    public user user { get; set; }

    public osm()
    {
      this.versionField = 0.600000023841858;
      this.generatorField = "OsmSharp";
    }
  }
}
