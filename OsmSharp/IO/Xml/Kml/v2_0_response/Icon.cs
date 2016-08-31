using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class Icon
  {
    private string hrefField;
    private int hField;
    private bool hFieldSpecified;
    private int wField;
    private bool wFieldSpecified;
    private int xField;
    private bool xFieldSpecified;
    private int yField;
    private bool yFieldSpecified;
    private int refreshIntervalField;
    private bool refreshIntervalFieldSpecified;
    private refreshMode refreshModeField;
    private bool refreshModeFieldSpecified;
    private viewRefreshMode viewRefreshModeField;
    private bool viewRefreshModeFieldSpecified;
    private int viewRefreshTimeField;
    private bool viewRefreshTimeFieldSpecified;
    private double viewBoundScaleField;
    private bool viewBoundScaleFieldSpecified;
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

    public int h
    {
      get
      {
        return this.hField;
      }
      set
      {
        this.hField = value;
      }
    }

    [XmlIgnore]
    public bool hSpecified
    {
      get
      {
        return this.hFieldSpecified;
      }
      set
      {
        this.hFieldSpecified = value;
      }
    }

    public int w
    {
      get
      {
        return this.wField;
      }
      set
      {
        this.wField = value;
      }
    }

    [XmlIgnore]
    public bool wSpecified
    {
      get
      {
        return this.wFieldSpecified;
      }
      set
      {
        this.wFieldSpecified = value;
      }
    }

    public int x
    {
      get
      {
        return this.xField;
      }
      set
      {
        this.xField = value;
      }
    }

    [XmlIgnore]
    public bool xSpecified
    {
      get
      {
        return this.xFieldSpecified;
      }
      set
      {
        this.xFieldSpecified = value;
      }
    }

    public int y
    {
      get
      {
        return this.yField;
      }
      set
      {
        this.yField = value;
      }
    }

    [XmlIgnore]
    public bool ySpecified
    {
      get
      {
        return this.yFieldSpecified;
      }
      set
      {
        this.yFieldSpecified = value;
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

    public double viewBoundScale
    {
      get
      {
        return this.viewBoundScaleField;
      }
      set
      {
        this.viewBoundScaleField = value;
      }
    }

    [XmlIgnore]
    public bool viewBoundScaleSpecified
    {
      get
      {
        return this.viewBoundScaleFieldSpecified;
      }
      set
      {
        this.viewBoundScaleFieldSpecified = value;
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
