using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0_response
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class StyleBlinker
  {
    private int numCyclesField;
    private OsmSharp.IO.Xml.Kml.v2_0_response.State[] stateField;
    private string idField;

    public int numCycles
    {
      get
      {
        return this.numCyclesField;
      }
      set
      {
        this.numCyclesField = value;
      }
    }

    [XmlElement("State")]
    public OsmSharp.IO.Xml.Kml.v2_0_response.State[] State
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
