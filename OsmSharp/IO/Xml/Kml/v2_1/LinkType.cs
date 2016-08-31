using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [XmlInclude(typeof (IconType))]
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("Icon", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class LinkType : ObjectType
  {
    private string hrefField;
    private refreshModeEnum refreshModeField;
    private float refreshIntervalField;
    private viewRefreshModeEnum viewRefreshModeField;
    private float viewRefreshTimeField;
    private float viewBoundScaleField;
    private string viewFormatField;
    private string httpQueryField;

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

    [DefaultValue(refreshModeEnum.onChange)]
    public refreshModeEnum refreshMode
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

    [DefaultValue(typeof (float), "4")]
    public float refreshInterval
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

    [DefaultValue(viewRefreshModeEnum.never)]
    public viewRefreshModeEnum viewRefreshMode
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

    [DefaultValue(typeof (float), "4")]
    public float viewRefreshTime
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

    [DefaultValue(typeof (float), "1")]
    public float viewBoundScale
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

    public string httpQuery
    {
      get
      {
        return this.httpQueryField;
      }
      set
      {
        this.httpQueryField = value;
      }
    }

    public LinkType()
    {
      this.refreshModeField = refreshModeEnum.onChange;
      this.refreshIntervalField = 4f;
      this.viewRefreshModeField = viewRefreshModeEnum.never;
      this.viewRefreshTimeField = 4f;
      this.viewBoundScaleField = 1f;
    }
  }
}
