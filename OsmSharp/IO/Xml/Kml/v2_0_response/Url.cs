using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class Url
  {
    private string hrefField;
    private refreshMode refreshModeField;
    private bool refreshModeFieldSpecified;
    private int refreshIntervalField;
    private bool refreshIntervalFieldSpecified;
    private viewRefreshMode viewRefreshModeField;
    private bool viewRefreshModeFieldSpecified;
    private int viewRefreshTimeField;
    private bool viewRefreshTimeFieldSpecified;
    private string viewFormatField;
    private string idField;

    [XmlElement(DataType = "anyURI")]
    public string href
    {
      get
      {
        return this.hrefField;
      }
      set
      {
        this.hrefField = value;
      }
    }

    public refreshMode refreshMode
    {
      get
      {
        return this.refreshModeField;
      }
      set
      {
        this.refreshModeField = value;
      }
    }

    [XmlIgnore]
    public bool refreshModeSpecified
    {
      get
      {
        return this.refreshModeFieldSpecified;
      }
      set
      {
        this.refreshModeFieldSpecified = value;
      }
    }

    public int refreshInterval
    {
      get
      {
        return this.refreshIntervalField;
      }
      set
      {
        this.refreshIntervalField = value;
      }
    }

    [XmlIgnore]
    public bool refreshIntervalSpecified
    {
      get
      {
        return this.refreshIntervalFieldSpecified;
      }
      set
      {
        this.refreshIntervalFieldSpecified = value;
      }
    }

    public viewRefreshMode viewRefreshMode
    {
      get
      {
        return this.viewRefreshModeField;
      }
      set
      {
        this.viewRefreshModeField = value;
      }
    }

    [XmlIgnore]
    public bool viewRefreshModeSpecified
    {
      get
      {
        return this.viewRefreshModeFieldSpecified;
      }
      set
      {
        this.viewRefreshModeFieldSpecified = value;
      }
    }

    public int viewRefreshTime
    {
      get
      {
        return this.viewRefreshTimeField;
      }
      set
      {
        this.viewRefreshTimeField = value;
      }
    }

    [XmlIgnore]
    public bool viewRefreshTimeSpecified
    {
      get
      {
        return this.viewRefreshTimeFieldSpecified;
      }
      set
      {
        this.viewRefreshTimeFieldSpecified = value;
      }
    }

    public string viewFormat
    {
      get
      {
        return this.viewFormatField;
      }
      set
      {
        this.viewFormatField = value;
      }
    }

    [XmlAttribute(DataType = "ID")]
    public string id
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
  }
}
