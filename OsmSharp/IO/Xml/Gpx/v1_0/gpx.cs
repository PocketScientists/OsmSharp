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
  [XmlRoot(IsNullable = false, Namespace = "http://www.topografix.com/GPX/1/0")]
  public class gpx
  {
    private string nameField;
    private string descField;
    private string authorField;
    private string emailField;
    private string urlField;
    private string urlnameField;
    private DateTime timeField;
    private bool timeFieldSpecified;
    private string keywordsField;
    private boundsType boundsField;
    private gpxWpt[] wptField;
    private gpxRte[] rteField;
    private gpxTrk[] trkField;
    private XElement[] anyField;
    private string versionField;
    private string creatorField;

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

    public string author
    {
      get
      {
        return this.authorField;
      }
      set
      {
        this.authorField = value;
      }
    }

    public string email
    {
      get
      {
        return this.emailField;
      }
      set
      {
        this.emailField = value;
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

    public string keywords
    {
      get
      {
        return this.keywordsField;
      }
      set
      {
        this.keywordsField = value;
      }
    }

    public boundsType bounds
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

    [XmlElement("wpt")]
    public gpxWpt[] wpt
    {
      get
      {
        return this.wptField;
      }
      set
      {
        this.wptField = value;
      }
    }

    [XmlElement("rte")]
    public gpxRte[] rte
    {
      get
      {
        return this.rteField;
      }
      set
      {
        this.rteField = value;
      }
    }

    [XmlElement("trk")]
    public gpxTrk[] trk
    {
      get
      {
        return this.trkField;
      }
      set
      {
        this.trkField = value;
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
    public string version
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

    [XmlAttribute]
    public string creator
    {
      get
      {
        return this.creatorField;
      }
      set
      {
        this.creatorField = value;
      }
    }

    public gpx()
    {
      this.versionField = "1.0";
    }
  }
}
