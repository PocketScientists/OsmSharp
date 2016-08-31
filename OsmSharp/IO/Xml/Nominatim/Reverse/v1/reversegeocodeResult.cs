using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Nominatim.Reverse.v1
{
  [GeneratedCode("xsd", "4.0.30319.1")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  public class reversegeocodeResult
  {
    private string place_idField;
    private string osm_typeField;
    private string osm_idField;
    private string latField;
    private string lonField;
    private string valueField;

    [XmlAttribute]
    public string place_id
    {
      get
      {
        return this.place_idField;
      }
      set
      {
        this.place_idField = value;
      }
    }

    [XmlAttribute]
    public string osm_type
    {
      get
      {
        return this.osm_typeField;
      }
      set
      {
        this.osm_typeField = value;
      }
    }

    [XmlAttribute]
    public string osm_id
    {
      get
      {
        return this.osm_idField;
      }
      set
      {
        this.osm_idField = value;
      }
    }

    [XmlAttribute]
    public string lat
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
    public string lon
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

    [XmlText]
    public string Value
    {
      get
      {
        return this.valueField;
      }
      set
      {
        this.valueField = value;
      }
    }
  }
}
