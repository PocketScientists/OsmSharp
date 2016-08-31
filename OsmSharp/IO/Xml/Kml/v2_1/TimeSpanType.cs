using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("TimeSpan", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class TimeSpanType : TimePrimitiveType
  {
    private string beginField;
    private string endField;

    public string begin
    {
      get
      {
        return this.beginField;
      }
      set
      {
        this.beginField = value;
      }
    }

    public string end
    {
      get
      {
        return this.endField;
      }
      set
      {
        this.endField = value;
      }
    }
  }
}
