using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_0
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(AnonymousType = true, Namespace = "http://earth.google.com/kml/2.0")]
  [XmlRoot(IsNullable = false, Namespace = "http://earth.google.com/kml/2.0")]
  public class begin
  {
    private TimeInstant timeInstantField;

    public TimeInstant TimeInstant
    {
      get
      {
        return this.timeInstantField;
      }
      set
      {
        this.timeInstantField = value;
      }
    }
  }
}
