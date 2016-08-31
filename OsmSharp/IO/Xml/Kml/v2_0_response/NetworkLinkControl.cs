using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class NetworkLinkControl
  {
    private string cookieField;
    private string linkDescriptionField;
    private string linkNameField;
    private string messageField;
    private int minRefreshPeriodField;
    private bool minRefreshPeriodFieldSpecified;
    private string idField;

    public string cookie
    {
      get
      {
        return this.cookieField;
      }
      set
      {
        this.cookieField = value;
      }
    }

    public string linkDescription
    {
      get
      {
        return this.linkDescriptionField;
      }
      set
      {
        this.linkDescriptionField = value;
      }
    }

    public string linkName
    {
      get
      {
        return this.linkNameField;
      }
      set
      {
        this.linkNameField = value;
      }
    }

    public string message
    {
      get
      {
        return this.messageField;
      }
      set
      {
        this.messageField = value;
      }
    }

    public int minRefreshPeriod
    {
      get
      {
        return this.minRefreshPeriodField;
      }
      set
      {
        this.minRefreshPeriodField = value;
      }
    }

    [XmlIgnore]
    public bool minRefreshPeriodSpecified
    {
      get
      {
        return this.minRefreshPeriodFieldSpecified;
      }
      set
      {
        this.minRefreshPeriodFieldSpecified = value;
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
