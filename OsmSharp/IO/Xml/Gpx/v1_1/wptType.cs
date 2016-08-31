using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Gpx.v1_1
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
  public class wptType
  {
    private Decimal eleField;
    private bool eleFieldSpecified;
    private DateTime timeField;
    private bool timeFieldSpecified;
    private Decimal magvarField;
    private bool magvarFieldSpecified;
    private Decimal geoidheightField;
    private bool geoidheightFieldSpecified;
    private string nameField;
    private string cmtField;
    private string descField;
    private string srcField;
    private linkType[] linkField;
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

    [XmlElement("link")]
    public linkType[] link
    {
      get
      {
        return this.linkField;
      }
      set
      {
        this.linkField = value;
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
