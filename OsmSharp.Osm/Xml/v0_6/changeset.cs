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
  public class changeset
  {
    private OsmSharp.Osm.Xml.v0_6.tag[] tagField;
    private long idField;
    private bool idFieldSpecified;
    private string userField;
    private long uidField;
    private bool uidFieldSpecified;
    private DateTime created_atField;
    private bool created_atFieldSpecified;
    private DateTime closed_atField;
    private bool closed_atFieldSpecified;
    private bool openField;
    private bool openFieldSpecified;
    private double min_latField;
    private bool min_latFieldSpecified;
    private double min_lonField;
    private bool min_lonFieldSpecified;
    private double max_latField;
    private bool max_latFieldSpecified;
    private double max_lonField;
    private bool max_lonFieldSpecified;

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
    public DateTime created_at
    {
      get
      {
        return this.created_atField;
      }
      set
      {
        this.created_atField = value;
      }
    }

    [XmlIgnore]
    public bool created_atSpecified
    {
      get
      {
        return this.created_atFieldSpecified;
      }
      set
      {
        this.created_atFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public DateTime closed_at
    {
      get
      {
        return this.closed_atField;
      }
      set
      {
        this.closed_atField = value;
      }
    }

    [XmlIgnore]
    public bool closed_atSpecified
    {
      get
      {
        return this.closed_atFieldSpecified;
      }
      set
      {
        this.closed_atFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public bool open
    {
      get
      {
        return this.openField;
      }
      set
      {
        this.openField = value;
      }
    }

    [XmlIgnore]
    public bool openSpecified
    {
      get
      {
        return this.openFieldSpecified;
      }
      set
      {
        this.openFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public double min_lat
    {
      get
      {
        return this.min_latField;
      }
      set
      {
        this.min_latField = value;
      }
    }

    [XmlIgnore]
    public bool min_latSpecified
    {
      get
      {
        return this.min_latFieldSpecified;
      }
      set
      {
        this.min_latFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public double min_lon
    {
      get
      {
        return this.min_lonField;
      }
      set
      {
        this.min_lonField = value;
      }
    }

    [XmlIgnore]
    public bool min_lonSpecified
    {
      get
      {
        return this.min_lonFieldSpecified;
      }
      set
      {
        this.min_lonFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public double max_lat
    {
      get
      {
        return this.max_latField;
      }
      set
      {
        this.max_latField = value;
      }
    }

    [XmlIgnore]
    public bool max_latSpecified
    {
      get
      {
        return this.max_latFieldSpecified;
      }
      set
      {
        this.max_latFieldSpecified = value;
      }
    }

    [XmlAttribute]
    public double max_lon
    {
      get
      {
        return this.max_lonField;
      }
      set
      {
        this.max_lonField = value;
      }
    }

    [XmlIgnore]
    public bool max_lonSpecified
    {
      get
      {
        return this.max_lonFieldSpecified;
      }
      set
      {
        this.max_lonFieldSpecified = value;
      }
    }
  }
}
