using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Gpx.v1_1
{
  [GeneratedCode("xsd", "2.0.50727.3038")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
  public class trksegType
  {
    private wptType[] trkptField;

    [XmlElement("trkpt")]
    public wptType[] trkpt
    {
      get
      {
        return this.trkptField;
      }
      set
      {
        this.trkptField = value;
      }
    }
  }
}
