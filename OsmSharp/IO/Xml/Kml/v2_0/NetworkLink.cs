using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class NetworkLink
  {
    private Url urlField;
    private bool flyToViewField;
    private bool flyToViewFieldSpecified;
    private string descriptionField;
    private string nameField;
    private bool refreshVisibilityField;
    private bool refreshVisibilityFieldSpecified;
    private bool visibilityField;
    private bool visibilityFieldSpecified;
    private string idField;

    public Url Url
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

    public bool flyToView
    {
      get
      {
        return this.flyToViewField;
      }
      set
      {
        this.flyToViewField = value;
      }
    }

    [XmlIgnore]
    public bool flyToViewSpecified
    {
      get
      {
        return this.flyToViewFieldSpecified;
      }
      set
      {
        this.flyToViewFieldSpecified = value;
      }
    }

    public string description
    {
      get
      {
        return this.descriptionField;
      }
      set
      {
        this.descriptionField = value;
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

    public bool refreshVisibility
    {
      get
      {
        return this.refreshVisibilityField;
      }
      set
      {
        this.refreshVisibilityField = value;
      }
    }

    [XmlIgnore]
    public bool refreshVisibilitySpecified
    {
      get
      {
        return this.refreshVisibilityFieldSpecified;
      }
      set
      {
        this.refreshVisibilityFieldSpecified = value;
      }
    }

    public bool visibility
    {
      get
      {
        return this.visibilityField;
      }
      set
      {
        this.visibilityField = value;
      }
    }

    [XmlIgnore]
    public bool visibilitySpecified
    {
      get
      {
        return this.visibilityFieldSpecified;
      }
      set
      {
        this.visibilityFieldSpecified = value;
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
