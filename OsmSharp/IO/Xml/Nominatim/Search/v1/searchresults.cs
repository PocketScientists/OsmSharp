using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Nominatim.Search.v1
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  [XmlRoot(IsNullable = false, Namespace = "")]
  public class searchresults
  {
    private searchresultsPlace[] placeField;
    private string timestampField;
    private string attributionField;
    private string querystringField;
    private string polygonField;
    private string exclude_place_idsField;
    private string more_urlField;

    [XmlElement("place", Form = XmlSchemaForm.Unqualified)]
    public searchresultsPlace[] place
    {
      get
      {
        return this.placeField;
      }
      set
      {
        this.placeField = value;
      }
    }

    [XmlAttribute]
    public string timestamp
    {
      get
      {
        return this.timestampField;
      }
      set
      {
        this.timestampField = value;
      }
    }

    [XmlAttribute]
    public string attribution
    {
      get
      {
        return this.attributionField;
      }
      set
      {
        this.attributionField = value;
      }
    }

    [XmlAttribute]
    public string querystring
    {
      get
      {
        return this.querystringField;
      }
      set
      {
        this.querystringField = value;
      }
    }

    [XmlAttribute]
    public string polygon
    {
      get
      {
        return this.polygonField;
      }
      set
      {
        this.polygonField = value;
      }
    }

    [XmlAttribute]
    public string exclude_place_ids
    {
      get
      {
        return this.exclude_place_idsField;
      }
      set
      {
        this.exclude_place_idsField = value;
      }
    }

    [XmlAttribute]
    public string more_url
    {
      get
      {
        return this.more_urlField;
      }
      set
      {
        this.more_urlField = value;
      }
    }
  }
}
