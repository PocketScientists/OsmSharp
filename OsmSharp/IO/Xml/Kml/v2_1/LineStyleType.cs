using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace OsmSharp.IO.Xml.Kml.v2_1
{
  [GeneratedCode("xsd", "2.0.50727.1432")]
  [DebuggerStepThrough]
  [XmlType(Namespace = "http://earth.google.com/kml/2.1")]
  [XmlRoot("LineStyle", IsNullable = false, Namespace = "http://earth.google.com/kml/2.1")]
  public class LineStyleType : ColorStyleType
  {
    private float widthField;

    [DefaultValue(typeof (float), "1")]
    public float width
    {
      get
      {
        return this.widthField;
      }
      set
      {
        this.widthField = value;
      }
    }

    public LineStyleType()
    {
      this.widthField = 1f;
    }
  }
}
