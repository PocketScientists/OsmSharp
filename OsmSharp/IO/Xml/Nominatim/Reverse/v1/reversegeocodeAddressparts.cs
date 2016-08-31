using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Nominatim.Reverse.v1
{
  [GeneratedCode("xsd", "4.0.30319.1")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  public class reversegeocodeAddressparts
  {
    private string house_numberField;
    private string roadField;
    private string suburbField;
    private string cityField;
    private string countyField;
    private string state_districtField;
    private string stateField;
    private string postcodeField;
    private string countryField;
    private string country_codeField;

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
    public string suburb
    {
      get
      {
        return this.suburbField;
      }
      set
      {
        this.suburbField = value;
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
  }
}
