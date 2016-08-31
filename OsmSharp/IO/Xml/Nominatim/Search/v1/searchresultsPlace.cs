using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Nominatim.Search.v1
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  public class searchresultsPlace
  {
    private string house_numberField;
    private string roadField;
    private string villageField;
    private string cityField;
    private string countyField;
    private string postcodeField;
    private string state_districtField;
    private string stateField;
    private string countryField;
    private string country_codeField;
    private string place_idField;
    private string osm_typeField;
    private string osm_idField;
    private string boundingboxField;
    private string polygonpointsField;
    private string latField;
    private string lonField;
    private string display_nameField;
    private string classField;
    private string typeField;

    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string house_number
    {
      get
      {
        return this.house_numberField;
      }
      set
      {
        this.house_numberField = value;
      }
    }

    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string road
    {
      get
      {
        return this.roadField;
      }
      set
      {
        this.roadField = value;
      }
    }

    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string village
    {
      get
      {
        return this.villageField;
      }
      set
      {
        this.villageField = value;
      }
    }

    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string city
    {
      get
      {
        return this.cityField;
      }
      set
      {
        this.cityField = value;
      }
    }

    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string county
    {
      get
      {
        return this.countyField;
      }
      set
      {
        this.countyField = value;
      }
    }

    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string postcode
    {
      get
      {
        return this.postcodeField;
      }
      set
      {
        this.postcodeField = value;
      }
    }

    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string state_district
    {
      get
      {
        return this.state_districtField;
      }
      set
      {
        this.state_districtField = value;
      }
    }

    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string state
    {
      get
      {
        return this.stateField;
      }
      set
      {
        this.stateField = value;
      }
    }

    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string country
    {
      get
      {
        return this.countryField;
      }
      set
      {
        this.countryField = value;
      }
    }

    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string country_code
    {
      get
      {
        return this.country_codeField;
      }
      set
      {
        this.country_codeField = value;
      }
    }

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
    public string boundingbox
    {
      get
      {
        return this.boundingboxField;
      }
      set
      {
        this.boundingboxField = value;
      }
    }

    [XmlAttribute]
    public string polygonpoints
    {
      get
      {
        return this.polygonpointsField;
      }
      set
      {
        this.polygonpointsField = value;
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

    [XmlAttribute]
    public string display_name
    {
      get
      {
        return this.display_nameField;
      }
      set
      {
        this.display_nameField = value;
      }
    }

    [XmlAttribute]
    public string @class
    {
      get
      {
        return this.classField;
      }
      set
      {
        this.classField = value;
      }
    }

    [XmlAttribute]
    public string type
    {
      get
      {
        return this.typeField;
      }
      set
      {
        this.typeField = value;
      }
    }
  }
}
