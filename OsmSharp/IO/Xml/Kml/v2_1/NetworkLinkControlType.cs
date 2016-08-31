using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  public class NetworkLinkControlType
  {
    private float minRefreshPeriodField;
    private string cookieField;
    private string messageField;
    private string linkNameField;
    private string linkDescriptionField;
    private SnippetType linkSnippetField;
    private string expiresField;
    private UpdateType updateField;
    private LookAtType lookAtField;

    [DefaultValue(typeof (float), "0")]
    public float minRefreshPeriod
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

    public SnippetType linkSnippet
    {
      get
      {
        return this.linkSnippetField;
      }
      set
      {
        this.linkSnippetField = value;
      }
    }

    public string expires
    {
      get
      {
        return this.expiresField;
      }
      set
      {
        this.expiresField = value;
      }
    }

    public UpdateType Update
    {
      get
      {
        return this.updateField;
      }
      set
      {
        this.updateField = value;
      }
    }

    public LookAtType LookAt
    {
      get
      {
        return this.lookAtField;
      }
      set
      {
        this.lookAtField = value;
      }
    }

    public NetworkLinkControlType()
    {
      this.minRefreshPeriodField = 0.0f;
    }
  }
}
