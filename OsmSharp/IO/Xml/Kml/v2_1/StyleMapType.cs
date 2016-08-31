using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("StyleMap", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class StyleMapType : StyleSelectorType
  {
    private StyleMapPairType[] pairField;

    [XmlElement("Pair")]
    public StyleMapPairType[] Pair
    {
      get
      {
        return this.pairField;
      }
      set
      {
        this.pairField = value;
      }
    }
  }
}
