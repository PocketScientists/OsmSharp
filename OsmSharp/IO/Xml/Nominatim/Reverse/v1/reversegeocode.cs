using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Nominatim.Reverse.v1
{
  [GeneratedCode("xsd", "4.0.30319.1")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true)]
  [XmlRoot(IsNullable = false, Namespace = "")]
  public class reversegeocode
  {
    private reversegeocodeResult[] resultField;
    private reversegeocodeAddressparts[] addresspartsField;
    private string timestampField;
    private string attributionField;
    private string querystringField;

    [XmlElement("result", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
    public reversegeocodeResult[] result
    {
      get
      {
        return this.resultField;
      }
      set
      {
        this.resultField = value;
      }
    }

    [XmlElement("addressparts", Form = XmlSchemaForm.Unqualified)]
    public reversegeocodeAddressparts[] addressparts
    {
      get
      {
        return this.addresspartsField;
      }
      set
      {
        this.addresspartsField = value;
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
  }
}
