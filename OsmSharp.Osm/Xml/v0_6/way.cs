using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.Osm.Xml.v0_6
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  [XmlRoot(IsNullable = false, Namespace = "")]
  public class way
  {
    private OsmSharp.Osm.Xml.v0_6.nd[] ndField;
    private OsmSharp.Osm.Xml.v0_6.tag[] tagField;
    private long idField;
    private bool idFieldSpecified;
    private string userField;
    private long uidField;
    private bool uidFieldSpecified;
    private bool visibleField;
    private bool visibleFieldSpecified;
    private ulong versionField;
    private bool versionFieldSpecified;
    private long changesetField;
    private bool changesetFieldSpecified;
    private DateTime timestampField;
    private bool timestampFieldSpecified;

    [XmlElement("nd")]
    public OsmSharp.Osm.Xml.v0_6.nd[] nd
    {
      get
      {
        return this.ndField;
      }
      set
      {
        this.ndField = value;
      }
    }

    [XmlElement("tag")]
    public OsmSharp.Osm.Xml.v0_6.tag[] tag
    {
      get
      {
        return this.tagField;
      }
      set
      {
        this.tagField = value;
      }
    }

    [XmlAttribute]
    public long id
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

    [XmlIgnore]
    public bool idSpecified
    {
      get
      {
        return this.idFieldSpecified;
      }
      set
      {
        this.idFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public string user
    {
      get
      {
        return this.userField;
      }
      set
      {
        this.userField = value;
      }
    }

    [XmlAttribute]
    public long uid
    {
      get
      {
        return this.uidField;
      }
      set
      {
        this.uidField = value;
      }
    }

    [XmlIgnore]
    public bool uidSpecified
    {
      get
      {
        return this.uidFieldSpecified;
      }
      set
      {
        this.uidFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public bool visible
    {
      get
      {
        return this.visibleField;
      }
      set
      {
        this.visibleField = value;
      }
    }

    [XmlIgnore]
    public bool visibleSpecified
    {
      get
      {
        return this.visibleFieldSpecified;
      }
      set
      {
        this.visibleFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public ulong version
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
    public long changeset
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

    [XmlIgnore]
    public bool changesetSpecified
    {
      get
      {
        return this.changesetFieldSpecified;
      }
      set
      {
        this.changesetFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public DateTime timestamp
    {
      get
      {
        return this.timestampField;
      }
      set
      {
        this.timestampField = value;
      }
    }

    [XmlIgnore]
    public bool timestampSpecified
    {
      get
      {
        return this.timestampFieldSpecified;
      }
      set
      {
        this.timestampFieldSpecified = value;
      }
    }
  }
}
