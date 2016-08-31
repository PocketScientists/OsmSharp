using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Gpx.v1_1
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
  public class copyrightType
  {
    private string yearField;
    private string licenseField;
    private string authorField;

    [XmlElement(DataType = "gYear")]
    public string year
    {
      get
      {
        return this.yearField;
      }
      set
      {
        this.yearField = value;
      }
    }

    [XmlElement(DataType = "anyURI")]
    public string license
    {
      get
      {
        return this.licenseField;
      }
      set
      {
        this.licenseField = value;
      }
    }

    [XmlAttribute]
    public string author
    {
      get
      {
        return this.authorField;
      }
      set
      {
        this.authorField = value;
      }
    }
  }
}
