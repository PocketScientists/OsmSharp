using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Gpx.v1_0
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://www.topografix.com/GPX/1/0")]
  public class gpxTrkTrksegTrkpt
  {
    private Decimal eleField;
    private bool eleFieldSpecified;
    private DateTime timeField;
    private bool timeFieldSpecified;
    private Decimal courseField;
    private bool courseFieldSpecified;
    private Decimal speedField;
    private bool speedFieldSpecified;
    private Decimal magvarField;
    private bool magvarFieldSpecified;
    private Decimal geoidheightField;
    private bool geoidheightFieldSpecified;
    private string nameField;
    private string cmtField;
    private string descField;
    private string srcField;
    private string urlField;
    private string urlnameField;
    private string symField;
    private string typeField;
    private fixType fixField;
    private bool fixFieldSpecified;
    private string satField;
    private Decimal hdopField;
    private bool hdopFieldSpecified;
    private Decimal vdopField;
    private bool vdopFieldSpecified;
    private Decimal pdopField;
    private bool pdopFieldSpecified;
    private Decimal ageofdgpsdataField;
    private bool ageofdgpsdataFieldSpecified;
    private string dgpsidField;
    private XElement[] anyField;
    private Decimal latField;
    private Decimal lonField;

    public Decimal ele
    {
      get
      {
        return this.eleField;
      }
      set
      {
        this.eleField = value;
      }
    }

    [XmlIgnore]
    public bool eleSpecified
    {
      get
      {
        return this.eleFieldSpecified;
      }
      set
      {
        this.eleFieldSpecified = value;
      }
    }

    public DateTime time
    {
      get
      {
        return this.timeField;
      }
      set
      {
        this.timeField = value;
      }
    }

    [XmlIgnore]
    public bool timeSpecified
    {
      get
      {
        return this.timeFieldSpecified;
      }
      set
      {
        this.timeFieldSpecified = value;
      }
    }

    public Decimal course
    {
      get
      {
        return this.courseField;
      }
      set
      {
        this.courseField = value;
      }
    }

    [XmlIgnore]
    public bool courseSpecified
    {
      get
      {
        return this.courseFieldSpecified;
      }
      set
      {
        this.courseFieldSpecified = value;
      }
    }

    public Decimal speed
    {
      get
      {
        return this.speedField;
      }
      set
      {
        this.speedField = value;
      }
    }

    [XmlIgnore]
    public bool speedSpecified
    {
      get
      {
        return this.speedFieldSpecified;
      }
      set
      {
        this.speedFieldSpecified = value;
      }
    }

    public Decimal magvar
    {
      get
      {
        return this.magvarField;
      }
      set
      {
        this.magvarField = value;
      }
    }

    [XmlIgnore]
    public bool magvarSpecified
    {
      get
      {
        return this.magvarFieldSpecified;
      }
      set
      {
        this.magvarFieldSpecified = value;
      }
    }

    public Decimal geoidheight
    {
      get
      {
        return this.geoidheightField;
      }
      set
      {
        this.geoidheightField = value;
      }
    }

    [XmlIgnore]
    public bool geoidheightSpecified
    {
      get
      {
        return this.geoidheightFieldSpecified;
      }
      set
      {
        this.geoidheightFieldSpecified = value;
      }
    }

    public string name
    {
      get
      {
        return this.nameField;
      }
      set
      {
        this.nameField = value;
      }
    }

    public string cmt
    {
      get
      {
        return this.cmtField;
      }
      set
      {
        this.cmtField = value;
      }
    }

    public string desc
    {
      get
      {
        return this.descField;
      }
      set
      {
        this.descField = value;
      }
    }

    public string src
    {
      get
      {
        return this.srcField;
      }
      set
      {
        this.srcField = value;
      }
    }

    [XmlElement(DataType = "anyURI")]
    public string url
    {
      get
      {
        return this.urlField;
      }
      set
      {
        this.urlField = value;
      }
    }

    public string urlname
    {
      get
      {
        return this.urlnameField;
      }
      set
      {
        this.urlnameField = value;
      }
    }

    public string sym
    {
      get
      {
        return this.symField;
      }
      set
      {
        this.symField = value;
      }
    }

    public string type
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

    public fixType fix
    {
      get
      {
        return this.fixField;
      }
      set
      {
        this.fixField = value;
      }
    }

    [XmlIgnore]
    public bool fixSpecified
    {
      get
      {
        return this.fixFieldSpecified;
      }
      set
      {
        this.fixFieldSpecified = value;
      }
    }

    [XmlElement(DataType = "nonNegativeInteger")]
    public string sat
    {
      get
      {
        return this.satField;
      }
      set
      {
        this.satField = value;
      }
    }

    public Decimal hdop
    {
      get
      {
        return this.hdopField;
      }
      set
      {
        this.hdopField = value;
      }
    }

    [XmlIgnore]
    public bool hdopSpecified
    {
      get
      {
        return this.hdopFieldSpecified;
      }
      set
      {
        this.hdopFieldSpecified = value;
      }
    }

    public Decimal vdop
    {
      get
      {
        return this.vdopField;
      }
      set
      {
        this.vdopField = value;
      }
    }

    [XmlIgnore]
    public bool vdopSpecified
    {
      get
      {
        return this.vdopFieldSpecified;
      }
      set
      {
        this.vdopFieldSpecified = value;
      }
    }

    public Decimal pdop
    {
      get
      {
        return this.pdopField;
      }
      set
      {
        this.pdopField = value;
      }
    }

    [XmlIgnore]
    public bool pdopSpecified
    {
      get
      {
        return this.pdopFieldSpecified;
      }
      set
      {
        this.pdopFieldSpecified = value;
      }
    }

    public Decimal ageofdgpsdata
    {
      get
      {
        return this.ageofdgpsdataField;
      }
      set
      {
        this.ageofdgpsdataField = value;
      }
    }

    [XmlIgnore]
    public bool ageofdgpsdataSpecified
    {
      get
      {
        return this.ageofdgpsdataFieldSpecified;
      }
      set
      {
        this.ageofdgpsdataFieldSpecified = value;
      }
    }

    [XmlElement(DataType = "integer")]
    public string dgpsid
    {
      get
      {
        return this.dgpsidField;
      }
      set
      {
        this.dgpsidField = value;
      }
    }

    [XmlAnyElement]
    public XElement[] Any
    {
      get
      {
        return this.anyField;
      }
      set
      {
        this.anyField = value;
      }
    }

    [XmlAttribute]
    public Decimal lat
    {
      get
      {
        return this.latField;
      }
      set
      {
        this.latField = value;
      }
    }

    [XmlAttribute]
    public Decimal lon
    {
      get
      {
        return this.lonField;
      }
      set
      {
        this.lonField = value;
      }
    }
  }
}
