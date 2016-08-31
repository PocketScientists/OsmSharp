using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Gpx.v1_0
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://www.topografix.com/GPX/1/0")]
  public class gpxRte
  {
    private string nameField;
    private string cmtField;
    private string descField;
    private string srcField;
    private string urlField;
    private string urlnameField;
    private string numberField;
    private XElement[] anyField;
    private gpxRteRtept[] rteptField;

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

    [XmlElement(DataType = "nonNegativeInteger")]
    public string number
    {
      get
      {
        return this.numberField;
      }
      set
      {
        this.numberField = value;
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

    [XmlElement("rtept")]
    public gpxRteRtept[] rtept
    {
      get
      {
        return this.rteptField;
      }
      set
      {
        this.rteptField = value;
      }
    }
  }
}
