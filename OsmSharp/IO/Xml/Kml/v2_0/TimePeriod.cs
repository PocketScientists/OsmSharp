using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class TimePeriod
  {
    private begin beginField;
    private end endField;
    private string idField;

    public begin begin
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

    public end end
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
